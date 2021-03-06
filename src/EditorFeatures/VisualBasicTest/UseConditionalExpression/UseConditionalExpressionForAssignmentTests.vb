﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic.UseConditionalExpression

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.UseConditionalExpression
    Partial Public Class UseConditionalExpressionForAssignmentTests
        Inherits AbstractVisualBasicDiagnosticProviderBasedUserDiagnosticTest

        Friend Overrides Function CreateDiagnosticProviderAndFixer(Workspace As Workspace) As (DiagnosticAnalyzer, CodeFixProvider)
            Return (New VisualBasicUseConditionalExpressionForAssignmentDiagnosticAnalyzer(),
                New VisualBasicUseConditionalExpressionForAssignmentCodeRefactoringProvider())
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnSimpleAssignment() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnSimpleAssignmentNoBlocks() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnSimpleAssignmentNoBlocks_NotInBlock() As Task

            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        if true
            [||]if true
                i = 0
            else
                i = 1
            end if
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        if true
            i = If(true, 0, 1)
        end if
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestNotOnSimpleAssignmentToDifferentTargets() As Task
            Await TestMissingAsync(
"
class C
    sub M(i as integer, j as integer)
        [||]if true
            i = 0
        else
            j = 1
        end if
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnAssignmentToUndefinedField() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        [||]if true
            me.i = 0
        else
            me.i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        me.i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnNonUniformTargetSyntax() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        [||]if true
            me.i = 0
        else
            me . i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        me.i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnAssignmentToDefinedField() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    dim i as integer

    sub M()
        [||]if true
            me.i = 0
        else
            me.i = 1
        end if
    end sub
end class",
"
class C
    dim i as integer

    sub M()
        me.i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnAssignmentToAboveLocalNoInitializer() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i as integer
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnAssignmentToAboveLocalLiteralInitializer() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i = 0
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestOnAssignmentToAboveLocalDefaultExpressionInitializer() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i as integer = nothing
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestDoNotMergeAssignmentToAboveLocalWithComplexInitializer() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i = Foo()
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = Foo()
        i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestDoNotMergeAssignmentToAboveLocalIfIntermediaryStatement() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i = 0
        Console.WriteLine()
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = 0
        Console.WriteLine()
        i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestDoNotMergeAssignmentToAboveIfLocalUsedInIfCondition() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i = 0
        [||]if Bar(i)
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = 0
        i = If(Bar(i), 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestDoNotMergeAssignmentToAboveIfMultiDecl() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim i = 0, j = 0
        [||]if true
            i = 0
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M()
        dim i = 0, j = 0
        i = If(true, 0, 1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestMissingWithoutElse() As Task
            Await TestMissingInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = 0
        end if
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestMissingWithoutElseWithStatementAfterwards() As Task
            Await TestMissingInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = 0
        end if

        i = 1
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestConversionWithUseDimForAll_CannotUseDimBecauseTypeWouldChange() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        ' keep object even though both values are strings
        dim o as object
        [||]if true
            o = ""a""
        else
            o = ""b""
        end if
    end sub
end class",
"
class C
    sub M()
        ' keep object even though both values are strings
        dim o as object = If(true, ""a"", ""b"")
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestConversionWithUseDimForAll_CanUseDimBecauseConditionalTypeMatches() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim s as string
        [||]if true
            s = ""a""
        else
            s = nothing
        end if
    end sub
end class",
"
class C
    sub M()
        dim s = If(true, ""a"", nothing)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestConversionWithUseDimForAll_CanUseDimButRequiresCastOfConditionalBranch() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M()
        dim s as string
        [||]if true
            s = nothing
        else
            s = nothing
        end if
    end sub
end class",
"
class C
    sub M()
        dim s = If(true, nothing, DirectCast(nothing, String))
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestKeepTriviaAroundIf() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        ' leading
        [||]if true
            i = 0
        else
            i = 1
        end if ' trailing
    end sub
end class",
"
class C
    sub M(i as integer)
        ' leading
        i = If(true, 0, 1) ' trailing
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestFixAll1() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        {|FixAllInDocument:if|} true
            i = 0
        else
            i = 1
        end if

        dim s as string
        if true
            s = ""a""
        else
            s = ""b""
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        i = If(true, 0, 1)

        dim s = If(true, ""a"", ""b"")
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestMultiLine1() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = Foo(
                1, 2, 3)
        else
            i = 1
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        i = If(true,
            Foo(
                1, 2, 3),
            1)
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestMultiLine2() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = 0
        else
            i = Foo(
                1, 2, 3)
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        i = If(true,
            0,
            Foo(
                1, 2, 3))
    end sub
end class")
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsUseConditionalExpression)>
        Public Async Function TestMultiLine3() As Task
            Await TestInRegularAndScriptAsync(
"
class C
    sub M(i as integer)
        [||]if true
            i = Foo(
                1, 2, 3)
        else
            i = Foo(
                4, 5, 6)
        end if
    end sub
end class",
"
class C
    sub M(i as integer)
        i = If(true,
            Foo(
                1, 2, 3),
            Foo(
                4, 5, 6))
    end sub
end class")
        End Function
    End Class
End Namespace
