﻿using System;

namespace MyCouch.Responses
{
#if !NETFX_CORE
    [Serializable]
#endif
    public class ContinuousChangesResponse : Response
    {
    }
}