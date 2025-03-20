using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ServerLogic
{
    private UdpClient server;
    private QuestionManager questionManager;
    private Dictionary<string, int> playerScores;
    private const int PORT = 11000;
    private const string RESULTS_FILE_PATH = @"D:\Test\ketqua.txt"; // Đường dẫn kết quả, có thể thay đổi đường dẫn tùy vào nơi lưu trữ

    public ServerLogic()
    {
        server = new UdpClient(PORT);
        questionManager = new QuestionManager();
        playerScores = new Dictionary<string, int>();
    }

    public async Task StartServer()
    {
        Console.WriteLine("Server started on port " + PORT);

        while (true)
        {
            try
            {
                // Nhan du lieu tu client
                UdpReceiveResult result = await server.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                IPEndPoint clientEndPoint = result.RemoteEndPoint;

                if (message == "REQUEST_QUESTION")
                {
                    // Gui cau hoi ngau nhien
                    Question question = questionManager.GetRandomQuestion();
                    string questionJson = JsonSerializer.Serialize(question);
                    byte[] responseData = Encoding.UTF8.GetBytes(questionJson);
                    await server.SendAsync(responseData, responseData.Length, clientEndPoint);
                }
                else if (message.StartsWith("ANSWER:"))
                {
                    // Xu ly cau tra loi
                    string[] parts = message.Split(':');

                    if (parts.Length < 2 || !int.TryParse(parts[1], out int answerId))
                    {
                        Console.WriteLine("Invalid answer format.");
                        return; // Hoac xu ly theo cach khac
                    }

                    string clientKey = clientEndPoint.ToString();

                    // Kiem tra neu nguoi choi chua co diem
                    if (!playerScores.ContainsKey(clientKey))
                        playerScores[clientKey] = 0;

                    // Kiem tra cau tra loi dung/sai


                    // Gui diem so ve client
                    string scoreMessage = $"SCORE:{playerScores[clientKey]}";
                    byte[] scoreData = Encoding.UTF8.GetBytes(scoreMessage);
                    await server.SendAsync(scoreData, scoreData.Length, clientEndPoint);

                    // Luu ket qua vao tep sau moi cau tra loi
                    SaveResultToFile(clientKey, playerScores[clientKey]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void SaveResultToFile(string clientKey, int score)
    {
        // Luu ket qua vao tep ketqua.txt
        string result = $"IP: {clientKey}, Diem: {score}, Thoi gian: {DateTime.Now}";

        try
        {
            string directory = Path.GetDirectoryName(RESULTS_FILE_PATH);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(RESULTS_FILE_PATH, append: true))
            {
                writer.WriteLine(result);
            }
            Console.WriteLine("Ket qua da duoc luu.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving result to file: {ex.Message}");
        }
    }
}
