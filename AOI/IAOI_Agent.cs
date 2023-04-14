namespace AOI
{
    public interface IAOI_Agent
    {
        /// <summary>
        ///  ����Agent����AOI����
        /// </summary>
        public void Enter( IAOI_Agent other_ );

        /// <summary>
        /// ��AOI�������ƶ�
        /// </summary>
        public void Move( IAOI_Agent other_ );

        /// <summary>
        /// ����Agent�뿪��AOI����
        /// </summary>
        public void Exit( IAOI_Agent other_ );
    }

}