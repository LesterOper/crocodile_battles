using App.Crocodile.Handlers;
using App.Crocodile.Interfaces;
using App.Weapon.WeaponBases;

namespace App.Crocodile
{
    public class CrocodileInfo
    {
        private  CrocodileMoverHandler _moverHandler;
        private  CrocodileStatsHandler _statsHandler;
        private  ICrocodile _crocodile;
        private  WeaponEquipingHandler _equipingHandler;
        private  BathFastenersHandler bathFastenersHandler;
        private bool longDistanceImmun;
        private bool closeDistanceImmun;

        public BathFastenersHandler BathFastenersHandler
        {
            get => bathFastenersHandler;
            set => bathFastenersHandler = value;
        }

        public bool LongDistanceImmun
        {
            get => longDistanceImmun;
            set => longDistanceImmun = value;
        }

        public bool CloseDistanceImmun
        {
            get => closeDistanceImmun;
            set => closeDistanceImmun = value;
        }
        
        public CrocodileMoverHandler MoverHandler
        {
            get => _moverHandler;
            set => _moverHandler = value;
        }

        public CrocodileStatsHandler StatsHandler
        {
            get => _statsHandler;
            set => _statsHandler = value;
        }

        public ICrocodile Crocodile
        {
            get => _crocodile;
            set => _crocodile = value;
        }

        public WeaponEquipingHandler EquipingHandler
        {
            get => _equipingHandler;
            set => _equipingHandler = value;
        }
    }
}