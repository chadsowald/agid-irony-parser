using Irony.Parsing;

namespace AgidIronyParser
{
    public class InflGrammer : Grammar
    {
        /*
        <line>             := <word><sp><pos>[?]:<sp><inflected forms>
        <word>             := [[A-Za-z']]+
        <sp>               := <literal space>
        <pos>              := [[VNA]]
        <inflected forms>  := <inflected form><sp>|<sp>...<sp>|<sp><inflected form>
        <inflected form>   := <individual entry>,<sp>...,<sp><individual entry>
        <individual entry> := <word><word tags>[<sp><variant level>][<sp>{<explanation>}]
        <word tags>        := [~][<][!][?]
        <explanation>      := [<explanation text>][:<distinguishing number>]
        <explanation text> := [[A-Za-z'_/]]+        
        */

        public InflGrammer() : base(false)
        {
            //Terminals and non-terminals
            var line = new NonTerminal("line");
            var singular = new RegexBasedTerminal("singular", "[A-Za-z']+");
            var plural = new RegexBasedTerminal("plural", "[A-Za-z']+");
            var pos = new RegexBasedTerminal("pos", "[VNA]");
            var question = new NonTerminal("question");
            var inflectedForms = new NonTerminal("inflectedForms");
            var inflectedForm = new NonTerminal("inflectedForm");
            var individualEntry = new NonTerminal("individualEntry");
            var wordTags = new RegexBasedTerminal("wordTags", "[~<!?]*");
            var explanation = new NonTerminal("explanation");
            var explanationTextValue = new RegexBasedTerminal("explanationTextValue", @"\w+");
            var explanationText = new NonTerminal("explanationText");
            var explanationTextNumber = new NonTerminal("explanationTextNumber");
            var variantLevel = new NonTerminal("variantLevel");
            var distinguishingNumber = new NumberLiteral("distinguishingNumber");
            var variantLevelNumber = new NumberLiteral("variantLevelNumber");
            var wordTagsPart = new NonTerminal("wordTagsPart");

            //Nullables
            question.Rule = "?" | Empty;
            wordTagsPart.Rule = wordTags | Empty;
            explanationText.Rule = explanationTextValue | Empty;
            explanationTextNumber.Rule = ":" + distinguishingNumber | Empty;
            explanation.Rule = "{" + explanationText + explanationTextNumber + "}" | Empty;
            variantLevel.Rule = variantLevelNumber | Empty;

            individualEntry.Rule = plural + wordTagsPart + variantLevel + explanation;

            //Repeaters
            inflectedForm.Rule = MakePlusRule(inflectedForm, ToTerm(","), individualEntry);
            inflectedForms.Rule = MakePlusRule(inflectedForms, ToTerm("|"), inflectedForm);

            //Top level
            line.Rule = singular + pos + question + ": " + inflectedForms;

            Root = line;

            //These non-terminals needn't be shown in the expression tree.
            MarkTransient(wordTagsPart, variantLevel);
            MarkPunctuation("{", "}", ": ");
        }
    }
}