namespace Library.View
{
    public interface IInterpolator
    {
        float Apply(float a);
    }

    public class AccelerateInterpolator : IInterpolator
    {
        public float Apply(float a)
        {
            return a * a;
        }
    }
}