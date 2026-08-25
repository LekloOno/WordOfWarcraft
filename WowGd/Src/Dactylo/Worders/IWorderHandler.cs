namespace WowGd.Src.Dactylo.Worders;

public interface IWorderHandler :
    IWorderCharHandler,
    IWorderEraseHandler,
    IWorderEraseAllHandler,
    IWorderStreamHandler;