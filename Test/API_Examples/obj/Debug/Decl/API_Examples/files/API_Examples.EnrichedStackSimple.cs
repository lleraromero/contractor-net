// File API_Examples.EnrichedStackSimple.cs
// Automatically generated contract file.
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics.Contracts;
using System;

// Disable the "this variable is not used" warning as every field would imply it.
#pragma warning disable 0414
// Disable the "this variable is never assigned to".
#pragma warning disable 0067
// Disable the "this event is never assigned to".
#pragma warning disable 0649
// Disable the "this variable is never used".
#pragma warning disable 0169
// Disable the "new keyword not required" warning.
#pragma warning disable 0109
// Disable the "extern without DllImport" warning.
#pragma warning disable 0626
// Disable the "could hide other member" warning, can happen on certain properties.
#pragma warning disable 0108


namespace API_Examples
{
  public partial class EnrichedStackSimple
  {
    #region Methods and constructors
    public EnrichedStackSimple ()
    {
      Contract.Ensures (this.count == 0);
      Contract.Ensures (this.state == 1);
    }

    public void Pop ()
    {
      Contract.Ensures ((this.state - this.oldState) <= 0);
      Contract.Ensures (this.count <= 2147483646);
      Contract.Ensures (this.oldState <= 3);
      Contract.Ensures (this.oldState >= 2);
      Contract.Ensures (this.state <= 2);
      Contract.Ensures (this.state >= 1);
    }

    public void Push ()
    {
      Contract.Ensures ((this.oldState - this.state) <= 0);
      Contract.Ensures (this.count >= -2147483647);
      Contract.Ensures (this.oldState <= 2);
      Contract.Ensures (this.oldState >= 1);
      Contract.Ensures (this.state <= 3);
      Contract.Ensures (this.state >= 2);
    }
    #endregion

    #region Fields
    public static int capacity;
    public int count;
    public uint oldState;
    public uint state;
    #endregion
  }
}
