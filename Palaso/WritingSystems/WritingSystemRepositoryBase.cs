using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Palaso.Code;
using Palaso.Xml;

namespace Palaso.WritingSystems
{
	/// <summary>
	/// This class forms the bases for managing collections of WritingSystemDefinitions. WritingSystemDefinitions
	/// can be registered and then retrieved and deleted by Id. The preferred use when editting a WritingSystemDefinition stored
	/// in the WritingSystemRepository is to Get the WritingSystemDefinition in question and then to clone it either via the
	/// Clone method on WritingSystemDefinition or via the MakeDuplicate method on the WritingSystemRepository. This allows
	/// changes made to a WritingSystemDefinition to be registered back with the WritingSystemRepository via the Set method,
	/// or to be discarded by simply discarding the object.
	/// Internally the WritingSystemRepository uses the WritingSystemDefinition's StoreId property to establish the identity of
	/// a WritingSystemDefinition. This allows the user to change the Rfc646Tag components and thereby the Id of a
	/// WritingSystemDefinition and the WritingSystemRepository to update itself and the underlying store correctly.
	/// </summary>
	abstract public class WritingSystemRepositoryBase : IWritingSystemRepository
	{

		private readonly Dictionary<string, IWritingSystemDefinition> _writingSystems;
		private readonly Dictionary<string, DateTime> _writingSystemsToIgnore;

		protected Dictionary<string, string> _idChangeMap;

		public event WritingSystemIdChangedEventHandler WritingSystemIdChanged;
		public event WritingSystemDeleted WritingSystemDeleted;
		public event WritingSystemConflatedEventHandler WritingSystemConflated;
		protected bool Conflating{ get; private set; }

		/// <summary>
		/// Constructor, set the CompatibilityMode
		/// </summary>
		protected WritingSystemRepositoryBase(WritingSystemCompatibility compatibilityMode)
		{
			CompatibilityMode = compatibilityMode;
			_writingSystems = new Dictionary<string, IWritingSystemDefinition>(StringComparer.OrdinalIgnoreCase);
			_writingSystemsToIgnore = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
			_idChangeMap = new Dictionary<string, string>();
			//_sharedStore = LdmlSharedWritingSystemRepository.Singleton;
		}

		protected IDictionary<string, DateTime> WritingSystemsToIgnore
		{
			get
			{
				return _writingSystemsToIgnore;
			}
		}

		virtual public IWritingSystemDefinition CreateNew()
		{
			return new WritingSystemDefinition();
		}

		virtual protected LdmlDataMapper CreateLdmlAdaptor()
		{
			return new LdmlDataMapper();
		}

		virtual public void Conflate(string wsToConflate, string wsToConflateWith)
		{
			Conflating = true;
			if(WritingSystemConflated != null)
			{
				WritingSystemConflated(this, new WritingSystemConflatedEventArgs(wsToConflate, wsToConflateWith));
			}
			Remove(wsToConflate);
			Conflating = false;
		}

		virtual public void Remove(string identifier)
		{
			if (identifier == null)
			{
				throw new ArgumentNullException("identifier");
			}
			if (!_writingSystems.ContainsKey(identifier))
			{
				throw new ArgumentOutOfRangeException("identifier");
			}
			// Delete from us
			//??? Do we really delete or just mark for deletion?
			_writingSystems.Remove(identifier);
			_writingSystemsToIgnore.Remove(identifier);
			if (!Conflating && WritingSystemDeleted != null)
			{
				WritingSystemDeleted(this, new WritingSystemDeletedEventArgs(identifier));
			}
			//TODO: Could call the shared store to advise that one has been removed.
			//TODO: This may be useful if writing systems were reference counted.
		}

		abstract public string WritingSystemIdHasChangedTo(string id);

		virtual public void LastChecked(string identifier, DateTime dateModified)
		{
			if (_writingSystemsToIgnore.ContainsKey(identifier))
			{
				_writingSystemsToIgnore[identifier] = dateModified;
			}
			else
			{
				_writingSystemsToIgnore.Add(identifier, dateModified);
			}
		}

		protected void Clear()
		{
			_writingSystems.Clear();
		}

		public IWritingSystemDefinition MakeDuplicate(IWritingSystemDefinition definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			return definition.Clone();
		}

		public abstract bool WritingSystemIdHasChanged(string id);

		[Obsolete("Deprecated: use Contains instead")]
		public bool Exists(string identifier)
		{
			return Contains(identifier);
		}

		public bool Contains(string identifier)
		{
			return _writingSystems.ContainsKey(identifier);
		}

		public bool CanSet(IWritingSystemDefinition ws)
		{
			if (ws == null)
			{
				return false;
			}
			return !(_writingSystems.Keys.Any(id => id.Equals(ws.Id, StringComparison.OrdinalIgnoreCase)) &&
				ws.StoreID != _writingSystems[ws.Id].StoreID);
		}

		public virtual void  Set(IWritingSystemDefinition ws)
		{
			if (ws == null)
			{
				throw new ArgumentNullException("ws");
			}

			//Check if this is a new writing system with a conflicting id
			if (!CanSet(ws))
			{
				throw new ArgumentException(String.Format("Unable to set writing system '{0}' because this id already exists. Please change this writing system id before setting it.", ws.Id));
			}
			string oldId = _writingSystems.Where(kvp => kvp.Value.StoreID == ws.StoreID).Select(kvp => kvp.Key).FirstOrDefault();
			//??? How do we update
			//??? Is it sufficient to just set it, or can we not change the reference in case someone else has it too
			//??? i.e. Do we need a ws.Copy(WritingSystemDefinition)?
			if (!String.IsNullOrEmpty(oldId) && _writingSystems.ContainsKey(oldId))
			{
				_writingSystems.Remove(oldId);
			}
			_writingSystems[ws.Id] = ws;
			// If the writing system already has a local keyboard, probably it has just been created in some dialog,
			// and we should respect the one the user set...though this is a very unlikely scenario, as we probably
			// don't have a local setting for a WS that is just being created.
			IKeyboardDefinition keyboard;
			if (_localKeyboardSettings != null && ((WritingSystemDefinition) ws).RawLocalKeyboard == null
				&& _localKeyboardSettings.TryGetValue(ws.Id, out keyboard))
			{
				ws.LocalKeyboard = keyboard;
			}
			if (!String.IsNullOrEmpty(oldId) && (oldId != ws.Id))
			{
				UpdateIdChangeMap(oldId, ws.Id);
				if (WritingSystemIdChanged != null)
				{
					WritingSystemIdChanged(this, new WritingSystemIdChangedEventArgs(oldId, ws.Id));
				}
			}
			if (ws.StoreID != ws.Id)
			{
				ws.StoreID = ws.Id;
			}
		}

		protected void UpdateIdChangeMap(string oldId, string newId)
		{
			if (_idChangeMap.ContainsValue(oldId))
			{
				// if the oldid is in the value of key/value, then we can update the cooresponding key with the newId
				string keyToChange = _idChangeMap.Where(pair => pair.Value == oldId).First().Key;
				_idChangeMap[keyToChange] = newId;
			}
			else if (_idChangeMap.ContainsKey(oldId))
			{
				// if oldId is already in the dictionary, set the result to be newId
				_idChangeMap[oldId] = newId;
			}
		}

		protected void LoadIdChangeMapFromExistingWritingSystems()
		{
			_idChangeMap.Clear();
			foreach (var pair in _writingSystems)
			{
				_idChangeMap[pair.Key] = pair.Key;
			}
		}

		public string GetNewStoreIDWhenSet(IWritingSystemDefinition ws)
		{
			if (ws == null)
			{
				throw new ArgumentNullException("ws");
			}
			return String.IsNullOrEmpty(ws.StoreID) ? ws.Id : ws.StoreID;
		}

		public IWritingSystemDefinition Get(string identifier)
		{
			if (identifier == null)
			{
				throw new ArgumentNullException("identifier");
			}
			if (!_writingSystems.ContainsKey(identifier))
			{
				throw new ArgumentOutOfRangeException("identifier", String.Format("Writing system id '{0}' does not exist.", identifier));
			}
			return _writingSystems[identifier];
		}

		public int Count
		{
			get
			{
				return _writingSystems.Count;
			}
		}

		virtual public void Save()
		{
		}

		virtual protected void OnChangeNotifySharedStore(IWritingSystemDefinition ws)
		{
			DateTime lastDateModified;
			if (_writingSystemsToIgnore.TryGetValue(ws.Id, out lastDateModified) && ws.DateModified > lastDateModified)
				_writingSystemsToIgnore.Remove(ws.Id);
		}

		virtual protected void OnRemoveNotifySharedStore()
		{
		}

		virtual public IEnumerable<IWritingSystemDefinition> WritingSystemsNewerIn(IEnumerable<IWritingSystemDefinition> rhs)
		{
			if (rhs == null)
			{
				throw new ArgumentNullException("rhs");
			}
			var newerWritingSystems = new List<WritingSystemDefinition>();
			foreach (var ws in rhs)
			{
				Guard.AgainstNull(ws, "ws in rhs");
				if (_writingSystems.ContainsKey(ws.Bcp47Tag))
				{
					DateTime lastDateModified;
					if ((!_writingSystemsToIgnore.TryGetValue(ws.Bcp47Tag, out lastDateModified) || ws.DateModified > lastDateModified)
						&& (ws.DateModified > _writingSystems[ws.Bcp47Tag].DateModified))
					{
						newerWritingSystems.Add(ws.Clone());
					}
				}
			}
			return newerWritingSystems;
		}

		public IEnumerable<IWritingSystemDefinition> AllWritingSystems
		{
			get
			{
				return _writingSystems.Values;
			}
		}

		public IEnumerable<IWritingSystemDefinition> TextWritingSystems
		{
			get { return _writingSystems.Values.Where(ws => !ws.IsVoice); }
		}

		public IEnumerable<IWritingSystemDefinition> VoiceWritingSystems
		{
			get { return _writingSystems.Values.Where(ws => ws.IsVoice); }
		}

		public virtual void OnWritingSystemIDChange(IWritingSystemDefinition ws, string oldId)
		{
			_writingSystems[ws.Id] = ws;
			_writingSystems.Remove(oldId);
		}

		/// <summary>
		/// filters the list down to those that are texts (not audio), while preserving their order
		/// </summary>
		/// <param name="idsToFilter"></param>
		/// <returns></returns>
		public IEnumerable<string> FilterForTextIds(IEnumerable<string> idsToFilter)
		{
			var textIds = TextWritingSystems.Select(ws => ws.Id);
			return idsToFilter.Where(id => textIds.Contains(id));
		}

		public WritingSystemCompatibility CompatibilityMode { get; private set; }

		private Dictionary<string, IKeyboardDefinition> _localKeyboardSettings;

		/// <summary>
		/// Getter gets the XML string that represents the user preferred keyboard for each writing
		/// system.
		/// Setter sets the user preferred keyboards on the writing systems based on the passed in
		/// XML string.
		/// </summary>
		public string LocalKeyboardSettings
		{
			get
			{
				var root = new XElement("keyboards");
				foreach (var ws in AllWritingSystems)
				{
					// We don't want to call LocalKeyboard here, because that will come up with some default.
					// If RawLocalKeyboard is null, we have never typed in this WS.
					// By the time we do, the user may have installed one of the keyboards in KnownKeyboards,
					// or done a Send/Receive and obtained a better list of KnownKeyboards, and we can then
					// make a better first guess than we can now. Calling LocalKeyboard and persisting the
					// result would have the effect of making our current guess permanent.
					var kbd = ((WritingSystemDefinition) ws).RawLocalKeyboard;
					if (kbd == null)
						continue;
					root.Add(new XElement("keyboard",
						new XAttribute("ws", ws.Id),
						new XAttribute("layout", kbd.Layout),
						new XAttribute("locale", kbd.Locale)));
				}
				return root.ToString();
			}
			set
			{
				_localKeyboardSettings = null;
				if (string.IsNullOrWhiteSpace(value))
					return;
				var root = XElement.Parse(value);
				_localKeyboardSettings = new Dictionary<string, IKeyboardDefinition>();
				foreach (var kbd in root.Elements("keyboard"))
				{
					var keyboard = Keyboard.Controller.CreateKeyboardDefinition(
						GetAttributeValue(kbd, "layout"), GetAttributeValue(kbd, "locale"));
					_localKeyboardSettings[kbd.Attribute("ws").Value] = keyboard;
				}
				// We do it like this rather than looking up the writing system by the ws attribute so as not to force the
				// creation of any writing systems which may be in the local keyboard settings but not in the current repo.
				foreach (var ws in AllWritingSystems)
				{
					IKeyboardDefinition localKeyboard;
					if (_localKeyboardSettings.TryGetValue(ws.Id, out localKeyboard))
						ws.LocalKeyboard = localKeyboard;
				}
			}
		}

		/// <summary>
		/// Get the writing system that is most probably intended by the user, when input language changes to the specified layout and cultureInfo,
		/// given the indicated candidates, and that wsCurrent is the preferred result if it is a possible WS for the specified culture.
		/// wsCurrent is also returned if none of the candidates is found to match the specified inputs.
		/// See interface comment for intended usage information.
		/// Enhance JohnT: it may be helpful, if no WS has an exact match, to look for one where the culture prefix (before hyphen) matches,
		/// thus finding a WS that has a keyboard for the same language as the one the user selected.
		/// Could similarly match against WS ID's language ID, for WS's with no RawLocalKeyboard.
		/// Could use LocalKeyboard instead of RawLocalKeyboard, thus allowing us to find keyboards for writing systems where the
		/// local keyboard has not yet been determined. However, this would potentially establish a particular local keyboard for
		/// a user who has never typed in that writing system or configured a keyboard for it, nor even selected any text in it.
		/// In the expected usage of this library, there will be a RawLocalKeyboard for every writing system in which the user has
		/// ever typed or selected text. That should have a high probability of catching anything actually useful.
		/// </summary>
		/// <param name="layoutName"></param>
		/// <param name="cultureInfo"></param>
		/// <param name="wsCurrent"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public IWritingSystemDefinition GetWsForInputLanguage(string layoutName, CultureInfo cultureInfo, IWritingSystemDefinition wsCurrent,
			IWritingSystemDefinition[] options)
		{
			// See if the default is suitable.
			if (WsMatchesLayout(layoutName, wsCurrent) && WsMatchesCulture(cultureInfo, wsCurrent))
				return wsCurrent;
			IWritingSystemDefinition layoutMatch = null;
			IWritingSystemDefinition cultureMatch = null;
			foreach (var ws in options)
			{
				bool matchesCulture = WsMatchesCulture(cultureInfo, ws);
				if (WsMatchesLayout(layoutName, ws))
				{
					if (matchesCulture)
						return ws;
					if (layoutMatch == null || ws.Equals(wsCurrent))
						layoutMatch = ws;
				}
				if (matchesCulture && (cultureMatch == null || ws.Equals(wsCurrent)))
					cultureMatch = ws;
			}
			return layoutMatch ?? cultureMatch ?? wsCurrent;
		}

		bool WsMatchesLayout(string layoutName, IWritingSystemDefinition ws)
		{
			var wsd = ws as WritingSystemDefinition;
			return wsd != null && wsd.RawLocalKeyboard != null && wsd.RawLocalKeyboard.Layout == layoutName;
		}

		private bool WsMatchesCulture(CultureInfo cultureInfo, IWritingSystemDefinition ws)
		{
			var wsd = ws as WritingSystemDefinition;
			return wsd != null && wsd.RawLocalKeyboard != null && wsd.RawLocalKeyboard.Locale == cultureInfo.Name;
		}

		private string GetAttributeValue(XElement node, string attrName)
		{
			var attr = node.Attribute(attrName);
			if (attr == null)
				return "";
			return attr.Value;
		}
	}

	public class WritingSystemIdChangedEventArgs : EventArgs
	{
		public WritingSystemIdChangedEventArgs(string oldId, string newId)
		{
			OldId = oldId;
			NewId = newId;
		}
		public string OldId { get; private set; }
		public string NewId { get; private set; }
	}

	public class WritingSystemConflatedEventArgs:WritingSystemIdChangedEventArgs
	{
		public WritingSystemConflatedEventArgs(string oldId, string newId) : base(oldId, newId)
		{
		}
	}

	public class WritingSystemDeletedEventArgs : EventArgs
	{
		public WritingSystemDeletedEventArgs(string id)
		{
			Id = id;
		}
		public string Id { get; private set; }
	}
}
