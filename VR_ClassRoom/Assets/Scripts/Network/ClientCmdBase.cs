using Pr.ClientLib.Tcp;

public abstract class ClientCmdBase
{
    public abstract void msgParse(IClientNetReader netData);
}
