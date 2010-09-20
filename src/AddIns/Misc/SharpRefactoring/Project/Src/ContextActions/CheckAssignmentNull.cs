﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Offers inserting "if (a == null) return;" after "var a = *expr*"
	/// </summary>
	public class CheckAssignmentNull : ContextAction
	{
		public override string Title {
			get { return "Check for null"; }
		}
		
		public override bool IsAvailable(EditorContext context)
		{
			var cache = context.GetCached<CheckAssignmentCache>();
			return cache.IsActionAvailable;
		}
		
		public override void Execute(EditorContext context)
		{
			var cache = context.GetCached<CheckAssignmentCache>();
			
			var ifStatement = GenerateAstToInsert(cache.VariableName);

			var editor = context.Editor;
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, editor.Document.GetLineStartOffset(cache.ElementRegion.GetStart()));
			string code = cache.CodeGenerator.GenerateCode(ifStatement, indent);
			int insertOffset = editor.Document.GetLineEndOffset(cache.ElementRegion.GetEnd());
			editor.Document.Insert(insertOffset, code);
			editor.Caret.Offset = insertOffset + code.Length - 1;
		}
		
		AbstractNode GenerateAstToInsert(string variableName)
		{
			return new IfElseStatement(
				new BinaryOperatorExpression(new IdentifierExpression(variableName), BinaryOperatorType.Equality, new PrimitiveExpression(null)),
				new ReturnStatement(Expression.Null));
		}
	}
}
