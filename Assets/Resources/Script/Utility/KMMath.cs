namespace KMUtility.Math
{
	public static class KMMath
	{
		/// <summary> 指定した精度の数値に切り捨てします</summary>
		/// <param name="_dValue"> 丸め対象の倍精度浮動小数点数</param>
		/// <param name="_iDigits"> 戻り値の有効桁数の精度</param>
		/// <returns> _iDigits に等しい精度の数値に切り捨てられた数値</returns>
		public static double ToRoundDown(double _dValue, int _iDigits)
		{
			double dCoef = System.Math.Pow(10, _iDigits);
			return _dValue > 0 ? System.Math.Floor(_dValue * dCoef) / dCoef : System.Math.Ceiling(_dValue * dCoef) / dCoef;
		}

		/// <summary> 指定した精度の数値に切り捨てします</summary>
		/// <param name="_fValue"> 丸め対象の浮動小数点数</param>
		/// <param name="_iDigits"> 戻り値の有効桁数の精度</param>
		/// <returns> _iDigits に等しい精度の数値に切り捨てられた数値</returns>
		public static float ToRoundDown(float _fValue, int _iDigits) => (float)ToRoundDown((double)_fValue, _iDigits);
	}
}
