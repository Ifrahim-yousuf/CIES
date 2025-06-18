using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace facadefinal
{
    public class KeylogReaderFacade
    {
        private readonly DeviceInfoService _deviceService;
        private readonly EmployeeService _employeeService;
        private readonly KeyLogService _logService;
        private readonly ScoringService _scoringService;
        private readonly ViolationAnalyzer _violationAnalyzer;

        public KeylogReaderFacade(string connectionString)
        {
            _deviceService = new DeviceInfoService(connectionString);
            _employeeService = new EmployeeService(connectionString);
            _logService = new KeyLogService(connectionString);
            _scoringService = new ScoringService(connectionString);
            _violationAnalyzer = new ViolationAnalyzer(connectionString);
        }

        public void ProcessDevice(string uuidStr, string[] lines)
        {
            if (!Guid.TryParse(uuidStr, out Guid uuid)) return;

            bool hasDeviceInfo = lines.Length >= 5;
            string keystrokes = hasDeviceInfo && lines.Length > 5
                ? string.Join(Environment.NewLine, lines[5..])
                : string.Join(Environment.NewLine, lines);

            if (hasDeviceInfo && !_deviceService.Exists(uuid))
            {
                _deviceService.Insert(uuid, lines[0], lines[1], lines[2], lines[3]);
            }

            _employeeService.InsertIfNotExists(uuid);
            _scoringService.InsertDefaultScoreIfNotExists(uuid);

            if (!string.IsNullOrWhiteSpace(keystrokes))
            {
                _logService.InsertKeyLogs(uuid, keystrokes);
                int penalty = _violationAnalyzer.AnalyzeAndGetPenalty(keystrokes);
                _scoringService.DeductScore(uuid, penalty);
            }
        }
    }

}
