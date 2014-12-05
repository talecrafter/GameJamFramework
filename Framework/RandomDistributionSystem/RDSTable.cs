using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rds
{
	/// <summary>
	/// Holds a table of RDS objects. This class is "the randomizer" of the RDS.
	/// The Result implementation of the IRDSTable interface uses the RDSRandom class
	/// to determine which elements are hit.
	/// </summary>
	public class RDSTable : IRDSTable
	{
		#region CONSTRUCTORS
		/// <summary>
		/// Initializes a new instance of the <see cref="RDSTable"/> class.
		/// </summary>
		public RDSTable()
			: this(null, 1, 1, false, false, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RDSTable"/> class.
		/// </summary>
		/// <param name="contents">The contents.</param>
		/// <param name="count">The count.</param>
		/// <param name="probability">The probability.</param>
		public RDSTable(IEnumerable<IRDSObject> contents, int count, double probability)
			: this(contents, count, probability, false, false, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RDSTable"/> class.
		/// </summary>
		/// <param name="contents">The contents.</param>
		/// <param name="count">The count.</param>
		/// <param name="probability">The probability.</param>
		/// <param name="unique">if set to <c>true</c> any item of this table (or contained sub tables) can be in the result only once.</param>
		/// <param name="always">if set to <c>true</c> the probability is disabled and the result will always contain (count) entries of this table.</param>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		public RDSTable(IEnumerable<IRDSObject> contents, int count, double probability, bool unique, bool always, bool enabled)
		{
			if (contents != null)
				mcontents = contents.ToList();
			else
				ClearContents();
			rdsCount = count;
			rdsProbability = probability;
			rdsUnique = unique;
			rdsAlways = always;
			rdsEnabled = enabled;
		}
		#endregion

		#region EVENTS
		/// <summary>
		/// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
		/// This is the moment to modify any settings immediately before a result is calculated.
		/// </summary>
		public event EventHandler rdsPreResultEvaluation;
		/// <summary>
		/// Occurs when this RDSObject has been hit by the Result procedure.
		/// (This means, this object will be part of the result set).
		/// </summary>
		public event EventHandler rdsHit;
		/// <summary>
		/// Occurs after the result has been calculated and the result set is complete, but before
		/// the RDSTable's Result method exits.
		/// </summary>
		public event ResultEventHandler rdsPostResultEvaluation;

		/// <summary>
		/// Raises the <see cref="E:PreResultEvaluation"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public virtual void OnRDSPreResultEvaluation(EventArgs e)
		{
			if (rdsPreResultEvaluation != null) rdsPreResultEvaluation(this, e);
		}
		/// <summary>
		/// Raises the <see cref="E:Hit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public virtual void OnRDSHit(EventArgs e)
		{
			if (rdsHit != null) rdsHit(this, e);
		}
		/// <summary>
		/// Raises the <see cref="E:PostResultEvaluation"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public virtual void OnRDSPostResultEvaluation(ResultEventArgs e)
		{
			if (rdsPostResultEvaluation != null) rdsPostResultEvaluation(this, e);
		}
		#endregion

		#region COUNT
		/// <summary>
		/// The maximum number of entries expected in the Result. The final count of items in the result may be lower
		/// if some of the entries may return a null result (no drop)
		/// </summary>
		public int rdsCount { get; set; }
		#endregion

		#region CONTENTS
		/// <summary>
		/// Gets or sets the contents of this table
		/// </summary>
		public IEnumerable<IRDSObject> rdsContents
		{
			get { return mcontents; }
		}
		private List<IRDSObject> mcontents = null;

		/// <summary>
		/// Clears the contents.
		/// </summary>
		public virtual void ClearContents()
		{
			mcontents = new List<IRDSObject>();
		}

		/// <summary>
		/// Adds the given entry to contents collection.
		/// </summary>
		/// <param name="entry">The entry.</param>
		public virtual void AddEntry(IRDSObject entry)
		{
			mcontents.Add(entry);
			entry.rdsTable = this;
		}

		/// <summary>
		/// Adds a new entry to the contents collection and allows directly assigning of a probability for it.
		/// Use this signature if (for whatever reason) the base classes constructor does not support all
		/// constructors of RDSObject or if you implemented IRDSObject directly in your class and you need
		/// to (re)apply a new probability at the moment you add it to a RDSTable.
		/// NOTE: The probability given is written back to the given instance "entry".
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="probability">The probability.</param>
		public virtual void AddEntry(IRDSObject entry, double probability)
		{
			mcontents.Add(entry);
			entry.rdsProbability = probability;
			entry.rdsTable = this;
		}

		/// <summary>
		/// Adds a new entry to the contents collection and allows directly assigning of a probability and drop flags for it.
		/// Use this signature if (for whatever reason) the base classes constructor does not support all
		/// constructors of RDSObject or if you implemented IRDSObject directly in your class and you need
		/// to (re)apply a new probability and flags at the moment you add it to a RDSTable.
		/// NOTE: The probability, unique, always and enabled flags given are written back to the given instance "entry".
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="probability">The probability.</param>
		/// <param name="unique">if set to <c>true</c> this object can only occur once per result.</param>
		/// <param name="always">if set to <c>true</c> [always] this object will appear always in the result.</param>
		/// <param name="enabled">if set to <c>false</c> [enabled] this object will never be part of the result (even if it is set to always=true!).</param>
		public virtual void AddEntry(IRDSObject entry, double probability, bool unique, bool always, bool enabled)
		{
			mcontents.Add(entry);
			entry.rdsProbability = probability;
			entry.rdsUnique = unique;
			entry.rdsAlways = always;
			entry.rdsEnabled = enabled;
			entry.rdsTable = this;
		}

		/// <summary>
		/// Removes the given entry from the contents. If it is not part of the contents, an exception occurs.
		/// </summary>
		/// <param name="entry">The entry.</param>
		public virtual void RemoveEntry(IRDSObject entry)
		{
			mcontents.Remove(entry);
			entry.rdsTable = null;
		}

		/// <summary>
		/// Removes the entry at the given index position.
		/// If the index is out-of-range of the current contents collection, an exception occurs.
		/// </summary>
		/// <param name="index">The index.</param>
		public virtual void RemoveEntry(int index)
		{
			IRDSObject entry = mcontents[index];
			entry.rdsTable = null;
			mcontents.RemoveAt(index);
		}
		#endregion

		#region RESULT
		private List<IRDSObject> uniquedrops = new List<IRDSObject>();

		private void AddToResult(List<IRDSObject> rv, IRDSObject o)
		{
			if (!o.rdsUnique || !uniquedrops.Contains(o))
			{
				if (o.rdsUnique)
					uniquedrops.Add(o);

				if (!(o is RDSNullValue))
				{
					if (o is IRDSTable)
					{
						rv.AddRange(((IRDSTable)o).rdsResult);
					}
					else
					{
						// INSTANCECHECK
						// Check if the object to add implements IRDSObjectCreator.
						// If it does, call the CreateInstance() method and add its return value
						// to the result set. If it does not, add the object o directly.
						IRDSObject adder = o;
						if (o is IRDSObjectCreator)
							adder = ((IRDSObjectCreator)o).rdsCreateInstance();

						rv.Add(adder);
						o.OnRDSHit(EventArgs.Empty);
					}
				}
				else
					o.OnRDSHit(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the result. Calling this method will start the random pick process and generate the result.
		/// This result remains constant for the lifetime of this table object.
		/// Use the ResetResult method to clear the result and create a new one.
		/// </summary>
		public virtual IEnumerable<IRDSObject> rdsResult
		{
			get
			{
				// The return value, a list of hit objects
				List<IRDSObject> rv = new List<IRDSObject>();
				uniquedrops = new List<IRDSObject>();

				// Do the PreEvaluation on all objects contained in the current table
				// This is the moment where those objects might disable themselves.
				foreach (IRDSObject o in mcontents)
					o.OnRDSPreResultEvaluation(EventArgs.Empty);

				// Add all the objects that are hit "Always" to the result
				// Those objects are really added always, no matter what "Count"
				// is set in the table! If there are 5 objects "always", those 5 will
				// drop, even if the count says only 3.
				foreach (IRDSObject o in mcontents.Where(e => e.rdsAlways && e.rdsEnabled))
					AddToResult(rv, o);

				// Now calculate the real dropcount, this is the table's count minus the
				// number of Always-drops.
				// It is possible, that the remaining drops go below zero, in which case
				// no other objects will be added to the result here.
				int alwayscnt = mcontents.Count(e => e.rdsAlways && e.rdsEnabled);
				int realdropcnt = rdsCount - alwayscnt;

				// Continue only, if there is a Count left to be processed
				if (realdropcnt > 0)
				{
					for (int dropcount = 0; dropcount < realdropcnt; dropcount++)
					{
						// Find the objects, that can be hit now
						// This is all objects, that are Enabled and that have not already been added through the Always flag
						IEnumerable<IRDSObject> dropables = mcontents.Where(e => e.rdsEnabled && !e.rdsAlways);

						// This is the magic random number that will decide, which object is hit now
						double hitvalue = RDSRandom.GetDoubleValue(dropables.Sum(e => e.rdsProbability));

						// Find out in a loop which object's probability hits the random value...
						double runningvalue = 0;
						foreach (IRDSObject o in dropables)
						{
							// Count up until we find the first item that exceeds the hitvalue...
							runningvalue += o.rdsProbability;
							if (hitvalue < runningvalue)
							{
								// ...and the oscar goes too...
								AddToResult(rv, o);
								break;
							}
						}
					}
				}

				// Now give all objects in the result set the chance to interact with
				// the other objects in the result set.
				ResultEventArgs rea = new ResultEventArgs(rv);
				foreach (IRDSObject o in rv)
					o.OnRDSPostResultEvaluation(rea);

				// Return the set now
				return rv;
			}
		}
		#endregion

		#region IRDSObject Members
		/// <summary>
		/// Gets or sets the probability for this object to be (part of) the result
		/// </summary>
		public double rdsProbability { get; set; }
		/// <summary>
		/// Gets or sets whether this object may be contained only once in the result set
		/// </summary>
		public bool rdsUnique { get; set; }
		/// <summary>
		/// Gets or sets whether this object will always be part of the result set
		/// (Probability is ignored when this flag is set to true)
		/// </summary>
		public bool rdsAlways { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IRDSObject"/> is enabled.
		/// Only enabled entries can be part of the result of a RDSTable.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		public bool rdsEnabled { get; set; }
		/// <summary>
		/// Gets or sets the table this Object belongs to.
		/// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
		/// </summary>
		public RDSTable rdsTable { get; set; }
		#endregion

		#region TOSTRING
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return ToString(0);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="indentationlevel">The indentationlevel. 4 blanks at the beginning of each line for each level.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public string ToString(int indentationlevel)
		{
			string indent = "".PadRight(4 * indentationlevel, ' ');

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(indent + "(RDSTable){0} Entries:{1},Prob:{2},UAE:{3}{4}{5}{6}", 
				this.GetType().Name, mcontents.Count, rdsProbability,
				(rdsUnique ? "1" : "0"), (rdsAlways ? "1" : "0"), (rdsEnabled ? "1" : "0"), (mcontents.Count > 0 ? "\r\n" : ""));

			foreach (IRDSObject o in mcontents)
				sb.AppendLine(indent + o.ToString(indentationlevel + 1));

			return sb.ToString();
		}
		#endregion
	}
}
