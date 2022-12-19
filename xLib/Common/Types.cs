using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xLib.Transceiver;

namespace xLib
{
    public delegate void ActionAccessUI(xAction request, object arg);
    public delegate void ActionAccessUI<TRequest>(xAction action, TRequest arg);

    public delegate TResult xAction<TResult, TRequest>(TRequest request);
    public delegate void xAction<TRequest>(TRequest request);
    public delegate void xAction(object request);

    public delegate void UIAction<TContext, TRequest>(TContext context, TRequest request);

    //public delegate void Event<TArgument>(object obj, TArgument arg);
    public delegate void xEvent<TArgument>(object obj, TArgument arg);
    public delegate void xEvent<TObject, TArgument>(TObject obj, TArgument arg);
    public delegate void xEvent(object arg);

    public delegate void xEventChangeState<TObject, TState>(TObject obj, TState state);

    public struct xContent
    {
        public unsafe byte* Data;
        public int DataSize;
    }
}
