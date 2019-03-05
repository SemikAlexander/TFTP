using System.IO;
using System.Threading;
using Tftp.Net;

namespace TFTP
{
    class Client
    {
        private static AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);
        public ClientForm form;
        public void download(string ipaddress, string filename, string local_folder)
        {
            var client = new TftpClient(ipaddress);

            //Prepare a simple transfer (GET test.dat)
            var transfer = client.Download(filename);

            //Capture the events that may happen during the transfer
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);
            //Start the transfer and write the data that we're downloading into a memory stream
            FileStream stream = new FileStream(Path.Combine(local_folder, filename), FileMode.OpenOrCreate);
            transfer.Start(stream);
            //Wait for the transfer to finish
            TransferFinishedEvent.WaitOne();
        }

        public void upload(string ipaddress, string path_to_file)
        {
            FileInfo file = new FileInfo(path_to_file);
            var client = new TftpClient(ipaddress);
            var transfer = client.Upload(file.Name);
            //Capture the events that may happen during the transfer
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);
            //Start the transfer and write the data that we're downloading into a memory stream
            FileStream stream = new FileStream(path_to_file, FileMode.Open);
            transfer.Start(stream);
            //Wait for the transfer to finish
            TransferFinishedEvent.WaitOne();
        }

        void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            form.Log.Invoke("Transfer running. Progress: " + progress);
        }

        void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            form.Log.Invoke("Transfer failed: " + error);
            TransferFinishedEvent.Set();
        }

        void transfer_OnFinshed(ITftpTransfer transfer)
        {
            form.Log.Invoke("Transfer succeeded.");
            TransferFinishedEvent.Set();
        }
    }
}