// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.Visitors
{
	public class FindMemberVisitor : AbstractAstVisitor
	{
		Location start, end;
		ParametrizedNode member = null;
		
		public ParametrizedNode Member {
			get { return member; }
		}
		
		public FindMemberVisitor(Location start, Location end)
		{
			this.start = start;
			this.end = end;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			if ((methodDeclaration.Body.StartLocation < start) &&
			    (methodDeclaration.Body.EndLocation > end)) {
				this.member = methodDeclaration;
			}
			
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if ((propertyDeclaration.BodyStart < start) &&
			    (propertyDeclaration.BodyEnd > end)) {
				this.member = propertyDeclaration;
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if ((constructorDeclaration.Body.StartLocation < start) &&
			    (constructorDeclaration.Body.EndLocation > end)) {
				this.member = constructorDeclaration;
			}
			
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			if ((operatorDeclaration.Body.StartLocation < start) &&
			    (operatorDeclaration.Body.EndLocation > end)) {
				this.member = operatorDeclaration;
			}
			
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}
	}
}
