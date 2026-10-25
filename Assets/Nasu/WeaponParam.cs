namespace Gene
{
    [System.Serializable]
    public struct WeaponParam
    {
        public float attackPower;  // 攻撃力
        public float attackSpeed;  // 攻撃速度
        public float range;        // 射程 / 飛距離
        public bool isAcid;        // 酸性フラグ
        public float acidDamage;   // 酸ダメージ
        public float acidChance; // 0～1。0.25なら25%
        public ElementType element; // 属性 (Fire, Ice, Plasma 等)

        public WeaponParam(
            float attack,
            float speed,
            float range,
            bool acid,
            float acidDmg,
            ElementType elem = ElementType.Base,
            float acidChance = 0f)
        {
            attackPower = attack;
            attackSpeed = speed;
            this.range = range;

            isAcid = acid;
            acidDamage = acidDmg;
            this.acidChance = acidChance;

            element = elem;
        }
    }
}