using UnityEngine;

namespace Timeline
{
    [RequireComponent( typeof( SignalReceiver ) )]
    public class SignalReceiver : MonoBehaviour
    {
        /// <summary>
        /// ÊÕµ½timelineµÄsignal
        /// </summary>
        public void OnReceiveSignal_1()
        {
            Debug.Log( "<color=white>OnReceiveSignal   1</color>" );
        }

        public void OnReceiveSignal_2()
        {
            Debug.Log( "<color=white>OnReceiveSignal   2</color>" );
        }
    }
}
