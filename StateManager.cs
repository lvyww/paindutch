namespace TypeB
{
    public enum TypingState
    {
        typing,
        pause,
        ready,
        end
    }

    public enum RetypeType
    {
        first,

        retype,
        shuffle,
        wrongRetype,
    }
    public enum TxtSource
    {
       unchange,
        qq,
        clipboard,
        changeSheng,
        jbs,
        book,
        trainer
    }
    static internal class StateManager
    {
        static public string Version = "P250811";
        static public bool TextInput = false;

        static public bool ConfigLoaded = false;

        static public bool LastType = false;


        static public TypingState typingState = TypingState.ready;

    
        static public TxtSource txtSource = TxtSource.unchange;
        static public RetypeType retypeType = RetypeType.first;
        //       static public bool IsChangSheng = false;
    }
}
