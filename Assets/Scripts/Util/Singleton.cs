using System;
namespace AdaptiveCamera.Util{
    public class Singleton<T>{

        //Singleton Pattern
        private T _instance;
        private readonly object instancelock = new object();
        public T Instance {
            get{
                if(_instance == null) lock(instancelock) if(_instance == null) _instance = init();
                return _instance;
            }
        }
        private readonly Func<T> init;
        public Singleton(Func<T> init){
            this.init = init;
        }
    }

    public static class Singleton{
        public static Singleton<T> Make<T>() where T : new() {
            return new Singleton<T>(()=> new T());
        }
    }
}