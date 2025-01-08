using QuatLanguage.Core.CustomAttributes;
using QuatLanguage.Core.Engine;
using QuatLanguage.Core.Engine.Words;


namespace QuatLanguage
{
    [Word("ThreadSleep")]
    public class ThreadSleep : QuatWord
    {
        public ThreadSleep(string name) : base(name)
        {
        }

        public override void Evaluate(QuatContext context)
        {
            var timeMillis = context.PopVStack();
            Thread.Sleep((int)timeMillis);
        }
    }
}
