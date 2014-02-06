using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CSML {
	public class Complex {
		double re;
		double im;

		/// <summary>
		/// Contains the real part of a complex number.
		/// </summary>
		public double Re {
			get { return re; }
			set { re = value; }
		}

		/// <summary>
		/// Contains the imaginary part of a complex number.
		/// </summary>
		public double Im {
			get { return im; }
			set { im = value; }
		}

		/// <summary>
		/// Imaginary unit.
		/// </summary>
		public static Complex I { get { return new Complex(0, 1); } }

		/// <summary>
		/// Complex number zero.
		/// </summary>
		public static Complex Zero { get { return new Complex(0, 0); } }

		/// <summary>
		/// Complex number one.
		/// </summary>
		public static Complex One { get { return new Complex(1, 0); } }
		
		#region Constructors

		/// <summary>
		/// Inits complex number as (0, 0).
		/// </summary>
		public Complex() {
			Re = 0;
			Im = 0;
		}

		/// <summary>
		/// Inits complex number with imaginary part = 0.
		/// </summary>
		/// <param name="real_part"></param>
		public Complex(double real_part) {
			Re = real_part;
			Im = 0;
		}

		/// <summary>
		/// Inits complex number.
		/// </summary>
		/// <param name="imaginary_part"></param>
		/// <param name="real_part"></param>
		public Complex(double real_part, double imaginary_part) {
			Re = real_part;
			Im = imaginary_part;
		}

		/// <summary>
		/// Inits complex number from string like "a+bi".
		/// </summary>
		/// <param name="s"></param>
		public Complex(string s) {
			while(s.Contains(" ")) s = removeAt(s,s.IndexOf(' '));
			if (s.Contains("*")) s = removeAt(s, s.IndexOf('*'));
			if (s.Contains("+")) {
				string[] vals = s.Split('+');
				if (vals[0].Contains("i")) {
					vals[0] = removeAt(vals[0], vals[0].IndexOf('i'));
					Im = double.Parse(vals[0], System.Globalization.NumberStyles.Float);
					Re = double.Parse(vals[1], System.Globalization.NumberStyles.Float);
				} else {
					vals[1] = removeAt(vals[1], vals[1].IndexOf('i'));
					Im = double.Parse(vals[1], System.Globalization.NumberStyles.Float);
					Re = double.Parse(vals[0], System.Globalization.NumberStyles.Float);
				}
			} else if (s.Contains("-")) {
				string[] vals = s.Split('-');
				if (vals[0].Contains("i")) {
					vals[0] = removeAt(vals[0], vals[0].IndexOf('i'));
					Im = -double.Parse(vals[0], System.Globalization.NumberStyles.Float);
					Re = double.Parse(vals[1], System.Globalization.NumberStyles.Float);
				} else {
					vals[1] = removeAt(vals[1], vals[1].IndexOf('i'));
					Im = -double.Parse(vals[1], System.Globalization.NumberStyles.Float);
					Re = double.Parse(vals[0], System.Globalization.NumberStyles.Float);
				}
			} else {
				if (s.Contains("i")) {
					s = removeAt(s, s.IndexOf('i'));
					Im = double.Parse(s, System.Globalization.NumberStyles.Float);
					Re = 0;
				} else {
					Im = 0;
					Re = double.Parse(s, System.Globalization.NumberStyles.Float);
				}
			}
		}

		public static Complex[] array(double[] dat) {
			Complex[] ret = new Complex[dat.Length];
			for (int i = 0; i < dat.Length; i++) ret[i] = new Complex(dat[i]);
			return ret;
		}

		private string removeAt(string s, int idx) { return s.Substring(0, idx) + s.Substring(idx + 1, s.Length - idx - 1); }

		public static Match Test(string s) {
			string dp = "([0-9]+[.]?[0-9]*|[.][0-9]+)";
			string dm = "[-]?" + dp;

			Regex r = new Regex("^(?<RePart>(" + dm + ")[-+](?<ImPart>(" + dp + "))[i])$");

			return r.Match(s);
		}

		#endregion

		#region Operators

		public static Complex operator +(Complex a, Complex b) {
			//if (a == null) return b;
			//else if (b == null) return a;
			//else 
			return new Complex(a.Re + b.Re, a.Im + b.Im);
		}

		public static Complex operator +(Complex a, double b) { return new Complex(a.Re + b, a.Im); }
		public static Complex operator +(double a, Complex b) { return new Complex(a + b.Re, b.Im); }
		public static Complex operator -(Complex a, Complex b) { return new Complex(a.Re - b.Re, a.Im - b.Im); }
		public static Complex operator -(Complex a, double b) { return new Complex(a.Re - b, a.Im); }
		public static Complex operator -(double a, Complex b) { return new Complex(a - b.Re, -b.Im); }
		public static Complex operator -(Complex a) { return new Complex(-a.Re, -a.Im); }
		public static Complex operator *(Complex a, Complex b) { return new Complex(a.Re * b.Re - a.Im * b.Im, a.Im * b.Re + a.Re * b.Im); }
		public static Complex operator *(Complex a, double d) { return new Complex(d) * a; }
		public static Complex operator *(double d, Complex a) { return new Complex(d) * a; }
		public static Complex operator /(Complex a, Complex b) { return a * Conj(b) * (1 / (Abs(b) * Abs(b))); }
		public static Complex operator /(Complex a, double b) { return a * (1 / b); }
		public static Complex operator /(double a, Complex b) { return a * Conj(b) * (1 / (Abs(b) * Abs(b))); }
		public static Complex operator ^(double a, Complex b) { return Complex.Pow(a, b); }
		public static Complex operator ^(Complex a, double b) { return Complex.Pow(a, b); }
		public static Complex operator ^(Complex a, Complex b) { return Complex.Pow(a, b); }
		public static bool operator ==(Complex a, Complex b) { return (a.Re == b.Re && a.Im == b.Im); }
		public static bool operator ==(Complex a, double b) { return a == new Complex(b); }
		public static bool operator ==(double a, Complex b) { return new Complex(a) == b; }
		public static bool operator !=(Complex a, Complex b) { return !(a == b); }
		public static bool operator !=(Complex a, double b) { return !(a == b); }
		public static bool operator !=(double a, Complex b) { return !(a == b); }

		#endregion

		#region Static funcs & overrides

		/// <summary>Gets only the Real values of the array of Complex</summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double[] Reals(Complex[] a) {
			double[] ret = new double[a.Length];
			for (int i = 0; i < a.Length; i++) ret[i] = a[i].Re;
			return ret;
		}

		/// <summary>Gets only the Imaginary values of the array of Complex</summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double[] Imags(Complex[] a) {
			double[] ret = new double[a.Length];
			for (int i = 0; i < a.Length; i++) ret[i] = a[i].Im;
			return ret;
		}

		/// <summary>Gets the absolute values of the array of Complex</summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double[] Abs(Complex[] a) {
			double[] ret = new double[a.Length];
			for (int i = 0; i < a.Length; i++) ret[i] = Abs(a[i]);
			return ret;
		}

		/// <summary>Gets the angles of the array of Complex</summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double[] Angle(Complex[] a) {
			double[] ret = new double[a.Length];
			for (int i = 0; i < a.Length; i++) ret[i] = Angle(a[i]);
			return ret;
		}

		/// <summary>Calculates the absolute value of a complex number</summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double Abs(Complex a) { return Math.Sqrt(a.Im * a.Im + a.Re * a.Re); }

		/// <summary>Calculates the angle of a complex number</summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double Angle(Complex a) { return Math.Atan2(a.Im, a.Re); }

		/// <summary>
		/// Inverts a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Inv(Complex a) { return new Complex(a.Re / (a.Re * a.Re + a.Im * a.Im), -a.Im / (a.Re * a.Re + a.Im * a.Im)); }

		/// <summary>
		/// Tangent of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Tan(Complex a) { return Sin(a) / Cos(a); }

		/// <summary>
		/// Hyperbolic cosine of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Cosh(Complex a) { return (Exp(a) + Exp(-a)) / 2; }

		/// <summary>
		/// Hyperbolic sine of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Sinh(Complex a) { return (Exp(a) - Exp(-a)) / 2; }

		/// <summary>
		/// Hyperbolic tangent of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Tanh(Complex a) { return (Exp(2 * a) - 1) / (Exp(2 * a) + 1); }

		/// <summary>
		/// Hyperbolic cotangent of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Coth(Complex a) { return (Exp(2 * a) + 1) / (Exp(2 * a) - 1); }

		/// <summary>
		/// Hyperbolic secant of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Sech(Complex a) { return Inv(Cosh(a)); }

		/// <summary>
		/// Hyperbolic cosecant of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Csch(Complex a) { return Inv(Sinh(a)); }

		/// <summary>
		/// Cotangent of a.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Cot(Complex a) { return Cos(a) / Sin(a); }

		/// <summary>
		/// Computes the conjugation of a complex number.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Conj(Complex a) { return new Complex(a.Re, -a.Im); }

		/// <summary>
		/// Complex square root.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static Complex Sqrt(double d) {
			if (d >= 0) return new Complex(Math.Sqrt(d));
			else return new Complex(0, Math.Sqrt(-d));
		}

		/// <summary>
		/// Complex square root.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>   
		public static Complex Sqrt(Complex a) {
			double tmp = Math.Sqrt(a.Re*a.Re + a.Im*a.Im);
			if (a.Im >= 0) return new Complex(Math.Sqrt((tmp + a.Re) / 2), Math.Sqrt((tmp - a.Re) / 2));
			return new Complex(Math.Sqrt((tmp + a.Re) / 2),-Math.Sqrt((tmp - a.Re) / 2));
		}

		/// <summary>
		/// Complex exponential function.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Exp(Complex a) { return new Complex(Math.Exp(a.Re) * Math.Cos(a.Im), Math.Exp(a.Re) * Math.Sin(a.Im)); }

		/// <summary>
		/// Main value of the complex logarithm.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Log(Complex a) {
			// Log[|w|]+I*(Arg[w]+2*Pi*k)            

			return new Complex(Math.Log(Abs(a)), Arg(a));
		}

		/// <summary>
		/// Argument of the complex number.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double Arg(Complex a) {
			if (a.Re < 0) {
				if (a.Im < 0) return Math.Atan(a.Im / a.Re) - Math.PI;
				else return Math.PI - Math.Atan(-a.Im / a.Re);
			} else return Math.Atan(a.Im / a.Re);
		}

		/// <summary>
		/// Complex cosine.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Cos(Complex a) { return .5 * (Exp(Complex.I * a) + Exp(-Complex.I * a)); }

		/// <summary>
		/// Complex sine.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static Complex Sin(Complex a) { return (Exp(Complex.I * a) - Exp(-Complex.I * a)) / (2 * Complex.I); }

		public static Complex Pow(Complex a, Complex b) { return Exp(b * Log(a)); }
		public static Complex Pow(double a, Complex b) { return Exp(b * Math.Log(a)); }
		public static Complex Pow(Complex a, double b) { return Exp(b * Log(a)); }

		/// <summary>
		/// a(1)*y(n) = b(1)*x(n) + b(2)*x(n-1) + ... + b(nb+1)*x(n-nb) - a(2)*y(n-1) - ... - a(na+1)*y(n-na)
		/// </summary>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static Complex[] Filter(Complex[] A, Complex[] B, Complex[] x) {
			Complex[] ret = new Complex[x.Length];
			for (int i = 0; i < ret.Length; i++) {
				ret[i] = Complex.Zero;
				for (int j = 0; j < B.Length; j++) { if (i - j < 0) break; ret[i] += B[j] * x[i - j]; }
				for (int j = 1; j < A.Length; j++) { if (i - j < 0) break; ret[i] -= A[j] * ret[i - j]; }
				ret[i] = ret[i] / A[0];
			}
			return ret;
		}


		public override string ToString() {
			if (this == Complex.Zero) return "0";

			string re, im, sign;

			if (this.Im < 0) {
				if (this.Re == 0) sign = "-";
				else sign = " - ";
			} else if (this.Im > 0 && this.Re != 0) sign = " + ";
			else sign = "";

			if (Math.Abs(this.Re) <= 0.00000001) re = "";
			else re = this.Re.ToString();

			if (Math.Abs(this.Im) <= 0.00000001) im = "";
			else if (this.Im == -1 || this.Im == 1) im = "i";
			else im = Math.Abs(this.Im).ToString() + "i";

			return re + sign + im;
		}

		public string ToString(string format) {
			if (this == Complex.Zero) return "0";
			else if (double.IsInfinity(this.Re) || double.IsInfinity(this.Im)) return "oo";
			else if (double.IsNaN(this.Re) || double.IsNaN(this.Im)) return "?";

			string re, im, sign;

			string imval;
			if (Math.Abs(Im) <= 0.00000001) imval = "0"; else imval = Math.Abs(this.Im).ToString(format);
			string reval;
			if (Math.Abs(Re) <= 0.00000001) reval = "0"; else reval = this.Re.ToString(format);

			if (imval.StartsWith("-")) {
				if (reval == "0") sign = "-";
				else sign = " - ";
			} else if (imval != "0" && reval != "0") {
				sign = " + ";
			} else sign = "";

			if (imval == "0") im = "";
			else if (imval == "1") im = "i";
			else im = imval + "i";

			if (reval == "0") {
				if (imval != "0") re = "";
				else re = "0";
			} else re = reval;

			return re + sign + im;
		}

		public override bool Equals(object obj) { return obj.ToString() == this.ToString(); }
		public override int GetHashCode() { return -1; }

		#endregion

		#region Dynamics

		public Complex Round() { return new Complex(Math.Round(this.Re), Math.Round(this.Im)); }
		public bool IsReal() { return (this.Im == 0); }
		public bool IsImaginary() { return (this.Re == 0); }

		#endregion
	}
}
