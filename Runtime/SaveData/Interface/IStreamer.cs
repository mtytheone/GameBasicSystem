using Cysharp.Threading.Tasks;
using System.Threading;

namespace HatzeLaboratory.GameBasicSystem.Runtime.SaveData.Interface
{
    public interface IStreamer
    {
        public void Save(byte[] data);
        public UniTask SaveAsync(byte[] data, CancellationToken cancellationtoken = default);
        public byte[] Load();
        public UniTask<byte[]> LoadAsync(CancellationToken cancellationtoken = default);
    }
}
