using Unity.Mathematics;
namespace AdaptiveCamera.Util{
    public class complex
    {
        public float2 value;
        public complex(float value) { this.value = new float2(value, 0); }
        public complex(float2 value) { this.value = value; }
        public complex(float real, float imaginary) { this.value = new float2(real, imaginary); }
        public static complex operator +(complex a, complex b) => new complex(a.value + b.value);
        public static complex operator -(complex a, complex b) => new complex(a.value - b.value);
        public static complex operator *(complex a, complex b){
            float2 reals = a.value * b.value;
            float2 complexes = a.value * b.value.yx;
            return new complex(reals.x - reals.y, complexes.x + complexes.y);
        }
        public static complex operator *(complex a, float b) => new complex(a.value *b);
        public static complex operator *(float a, complex b) => new complex(a *b.value);
        public complex Conjugate => new complex(this.value * new float2(1, -1));
        public static complex operator /(complex a, complex b) => a * b.Conjugate / math.lengthsq(b.value);
        public static complex operator /(complex a, float b) => new complex(a.value /b);
        public static complex operator /(float a, complex b) => a * b.Conjugate / math.lengthsq(b.value);

        public complex power(float b){
            var magnitude = math.pow(math.length(this.value),b);
            var angle = math.atan2(this.value.y, this.value.x)*b;
            return new complex(new float2(math.cos(angle), math.sin(angle)) * magnitude);
        }
        public complex Exp => new complex(math.cos(this.value.y), math.sin(this.value.y)) * math.exp(this.value.x);
        public complex Log => new complex(math.log(math.length(this.value)), math.atan2(this.value.y, this.value.x));
        public complex power(complex b) => (b * this.Log).Exp;
        public static complex unity(int b, int i){
            var angle = math.PI * 2 * i / b;
            return new complex(math.cos(angle), math.sin(angle));
        }
        public static complex operator -(complex a) => new complex(-a.value);
        public override string ToString(){
            return this.value.x.ToString() + " + " + this.value.y.ToString() + "i";
        }
    }
}