using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Tftp.Net;

namespace TFTP
{
    class Server
    {
        private static String ServerDirectory;
        private TftpServer tftpServer;
        ServerForm form = null;
        public void run(string dir, ServerForm _form)
        {
            form = _form;
            ServerDirectory = dir;
            Debug.Write("Server dir " + ServerDirectory);
            tftpServer = new TftpServer();
            {
                tftpServer.OnReadRequest += new TftpServerEventHandler(server_OnReadRequest);
                tftpServer.OnWriteRequest += new TftpServerEventHandler(server_OnWriteRequest);
                tftpServer.Start();
                form.Log.Invoke($"Server started!");
            }
        }
        public void stop()
        {
            tftpServer.Dispose();
        }
        private void server_OnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {
            String file = Path.Combine(ServerDirectory, transfer.Filename);

            if (File.Exists(file))
            {
                CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
            }
            else
            {
                OutputTransferStatus(transfer, "Accepting write request from " + client);
                StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
            }
        }

        private void server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            String path = Path.Combine(ServerDirectory, transfer.Filename);
            FileInfo file = new FileInfo(path);

            //Is the file within the server directory?
            if (!file.FullName.StartsWith(ServerDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
            }
            else if (!file.Exists)
            {
                CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
            }
            else
            {
                OutputTransferStatus(transfer, "Accepting request from " + client);
                StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open));
            }
        }

        private void StartTransfer(ITftpTransfer transfer, Stream stream)
        {
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinished);
            transfer.Start(stream);
        }

        private void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
        {
            OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
            transfer.Cancel(reason);
        }

        private void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            OutputTransferStatus(transfer, "Error: " + error);
        }

        private void transfer_OnFinished(ITftpTransfer transfer)
        {
            OutputTransferStatus(transfer, "Finished");
        }

        private void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            OutputTransferStatus(transfer, "Progress " + progress);
        }

        private void OutputTransferStatus(ITftpTransfer transfer, string message)
        {
            form.Log.Invoke("[" + transfer.Filename + "] " + message);
            Debug.Write("[" + transfer.Filename + "] " + message);
        }
    }
}