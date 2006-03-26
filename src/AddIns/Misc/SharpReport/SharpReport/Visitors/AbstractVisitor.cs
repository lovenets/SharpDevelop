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
using System.Xml;
	
using SharpReport.Designer;
	
using SharpReportCore;
namespace SharpReport.Visitors
{
	
	
	/// <summary>
	/// Abstract Class for Designer Visitor
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 02.12.2004 16:53:00
	/// </remarks>
	public class AbstractVisitor : object, SharpReport.Designer.IDesignerVisitor {
		private readonly string nodesQuery = "controls/control";	
		private string fileName;
		private XmlFormReader xmlFormReader;
		
		public AbstractVisitor() {
		}
		
		public AbstractVisitor(string fileName){
			this.fileName = fileName;
			xmlFormReader = new XmlFormReader() ;
		}
		
		
		
		/// <summary>
		/// All classes how use this baseclass have to override this method
		/// </summary>
		/// <remarks>
		/// Interface method from IDesignerVisitor
		/// 
		/// </remarks>
		public virtual void Visit(SharpReport.Designer.BaseDesignerControl designer) {
			
		}
		
	
		
		#region Properties
		protected string FileName {
			get {
				return fileName;
			}
		}
		
		protected XmlFormReader XmlFormReader {
			get {
				return xmlFormReader;
			}
		}
		
		protected string NodesQuery {
			get {
				return nodesQuery;
			}
		}
		
		#endregion
		
	}
}
