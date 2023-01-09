using UnityEngine;
using UnityEngine.UI;

namespace BoroGameDev.Utilities {

	public class ProgressBar : MonoBehaviour {
		public int Minimum;
		public int Maximum;
		public int Current;
		public Image Mask;
		public Image Fill;
		public Color FillColor;

		private void Update() {
			GetCurrentFill();
		}

		public void GetCurrentFill() {
			float currentOffset = Current - Minimum;
			float maximumOffset = Maximum - Minimum;
			float fillAmount = (float)currentOffset / (float)maximumOffset;
			Mask.fillAmount = fillAmount;

			Fill.color = FillColor;
		}
	}

}
