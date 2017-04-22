using System;
using UnityEngine;

namespace Assets.Currency
{
    public abstract class Currency
    {
        [SerializeField]
        private int _amount;
        private int _cachedUnlimitedCurrency;
        private bool _unlimitedCurrency;

        public virtual event Action AmountChanged;

        public bool UnlimitedCurrency
        {
            get { return _unlimitedCurrency; }
            set
            {
                if (value == _unlimitedCurrency) return;

                if (value) _cachedUnlimitedCurrency = _amount;
                else _amount = _cachedUnlimitedCurrency;

                _unlimitedCurrency = value;
                if (AmountChanged != null) AmountChanged();
            }
        }

        public int Amount
        {
            get { return UnlimitedCurrency ? int.MaxValue : _amount; }
            set
            {
                var oldValue = _amount;
                _amount = value;
                if (oldValue != value && AmountChanged != null)
                    AmountChanged();
            }
        }
        
        public Currency Init(int value)
        {
            _amount = value;
            return this;
        }
    }
}