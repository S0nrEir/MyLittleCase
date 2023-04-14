namespace AOI
{
    public interface IAOI_Agent
    {
        /// <summary>
        ///  其他Agent进入AOI区域
        /// </summary>
        public void Enter( IAOI_Agent other_ );

        /// <summary>
        /// 在AOI区域内移动
        /// </summary>
        public void Move( IAOI_Agent other_ );

        /// <summary>
        /// 其他Agent离开该AOI区域
        /// </summary>
        public void Exit( IAOI_Agent other_ );
    }

}