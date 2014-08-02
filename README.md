TSqlFlex.SqlParser
==================

This is a simple T-SQL parser written in C#.

As of right now, only the tokenizer is working, but only for the most elementary of statements.

Currently, the best way to see how it works is to use the tests.

**example:**

    var tokenizerTask = SqlTokenizer.TokenizeAsync("SELECT * FROM MyTable");
    var sqlTokens = await tokenizerTask;
    for (int i = 0; i < sqlTokens.Count; i += 1)
    {
        Debug.Print(sqlTokens[i].TokenType.ToString());
    }
    /*
        Outputs:
        Select
        Whitespace
        Star
        Whitespace
        From
        Whitespace
        Unknown
    */
            

