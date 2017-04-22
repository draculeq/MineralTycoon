using UnityEngine;

namespace Assets.Currency
{
    public interface IWallet
    {
        Cash Cash { get; }
        ResearchPoints ResearchPoints { get; }
    }

    public class Wallet : MonoBehaviour, IWallet
    {
        [SerializeField]
        private Cash _cash;
        [SerializeField]
        private ResearchPoints _researchPoints;

        public Cash Cash
        {
            get { return _cash; }
        }

        public ResearchPoints ResearchPoints
        {
            get { return _researchPoints; }
        }

        public void Awake()
        {
            if (_cash != null)
                _cash = new Cash().Init(5000) as Cash;
            if (_cash != null)
                _researchPoints = new ResearchPoints().Init(5000) as ResearchPoints;
        }
    }
}
