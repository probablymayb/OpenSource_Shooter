using Photon.Realtime;

public interface IScenePhotonManager
{
    void Initialize();
    void Release();
    void InitializeSceneObject();
    void UpdatePlayerAvatar(Player player);
}
