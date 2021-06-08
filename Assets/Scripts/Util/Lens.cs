using System;
namespace AdaptiveCamera.Util
{
    public class Lens<T>
    {
        private Func<T> getter;
        private Action<T> setter;
        public Lens(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Get => getter();
        public void Set(T value) => setter(value);
    }
}