using System.Collections.Generic;

public class Chat
{
    public int Id { get; set; }
    public string User { get; set; }
    public Dictionary<string, string> Conversation { get; set; }
}

public class Question
{
    public string Value { get; set; }
}

public class Answer
{
    public string Value { get; set; }
}