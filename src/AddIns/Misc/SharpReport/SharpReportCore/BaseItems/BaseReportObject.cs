//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------


using System;
using System.Xml.Serialization;
	
using System.ComponentModel;
using System.Drawing;
	
/// <summary>
/// This is the BaseClass for all 
///  <see cref="ReportItem"></see> 
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 18.08.2005 13:59:11
/// </remarks>
	
namespace SharpReportCore {	
	public class BaseReportObject : IBaseRenderer,INotifyPropertyChanged,
									IDisposable{
		
		private string name;
		private object parent;
		private bool visible = true;
		private bool  canGrow ;
		private bool canShrink ;
		private bool pageBreakBefore;
		private bool pageBreakAfter;
		private bool suspend;
		private Size size;
		private Point location;
		
		private Color backColor;
		private int  sectionOffset;

		public event EventHandler<EventArgs> BeforePrinting;
		public event EventHandler<AfterPrintEventArgs> AfterPrinting;
		
		
		#region SharpReportCore.IPropertyChange interface implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
		
		public BaseReportObject() {
			
		}
	
		
		protected void NotifyPropertyChanged(string property) {
			if (!suspend) {
				if (this.PropertyChanged != null) {
					this.PropertyChanged(this,new PropertyChangedEventArgs (property));
				}
			}
		}
		public void SuspendLayout () {
			suspend = true;
		}

		public void ResumeLayout () {
			suspend = false;
		}
		
		#region properties
		public virtual bool Visible {
			get {
				return visible;
			}
			set {
				visible = value;
				NotifyPropertyChanged ("Visible");
			}
		}
		
		public virtual bool CanGrow {
			get {
				return canGrow;
			}
			set {
				canGrow = value;
				NotifyPropertyChanged ("CanGrow");
			}
		}
		public virtual bool CanShrink {
			get {
				return canShrink;
			}
			set {
				canShrink = value;
				NotifyPropertyChanged ("CanShrink");
			}
		}
		
		
		
		public virtual string Name {
			get {
				return name;
			}
			set {
				name = value;
				NotifyPropertyChanged ("Name");
			}
		}
		
		public virtual bool PageBreakAfter {
			get {
				return pageBreakAfter;
			}
			set {
				pageBreakAfter = value;
				NotifyPropertyChanged ("PageBreakAfter");
			}
		}
		
		
		public virtual bool PageBreakBefore {
			get {
				return pageBreakBefore;
			}
			set {
				pageBreakBefore = value;
				NotifyPropertyChanged ("PageBreakBefore");
			}
		}
		
		
		
		public virtual Size Size {
			get {
				return size;
			}
			set {
				size = value;
				NotifyPropertyChanged ("Size");
			}
		}
		
		public virtual Point Location {
			get {
				return location;
			}
			set {
				location = value;
				NotifyPropertyChanged ("Location");
			}
		}
		
		public virtual Color BackColor {
			get {
				return backColor;
			}
			set {
				backColor = value;
				NotifyPropertyChanged ("BackColor");
			}
		}
		
		[XmlIgnoreAttribute]
		[Browsable(false)]
		public virtual int SectionOffset {
			get {
				return sectionOffset;
			}
			set {
				sectionOffset = value;
			}
		}
		
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public virtual object Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		
		[XmlIgnoreAttribute]
		[Browsable(false)]
		public bool Suspend {
			get {
				return suspend;
			}
		}
		#endregion
		
		#region EventHandling
		public void NotiyfyAfterPrint (PointF afterPrintLocation) {
			if (this.AfterPrinting != null) {
				AfterPrintEventArgs rea = new AfterPrintEventArgs (afterPrintLocation);
				AfterPrinting(this, rea);
			}
		}
		
		public void NotifyBeforePrint () {
			if (this.BeforePrinting != null) {
				BeforePrinting (this,EventArgs.Empty);
			}
		}
		
		#endregion
		
		#region SharpReportCore.IBaseRenderer interface implementation
		public virtual void  Render(ReportPageEventArgs rpea) {
			
		}
		#endregion
		
		#region IDisposable
		
		public virtual void Dispose () {
			Dispose(true);
            GC.SuppressFinalize(this);
		}
		
		~BaseReportObject(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing) {
			if (disposing){
			
			}
		}

		#endregion
	}
}
