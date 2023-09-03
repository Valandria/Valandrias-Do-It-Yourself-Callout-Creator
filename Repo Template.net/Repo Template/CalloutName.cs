using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Repo_Template
{
    [CalloutProperties("CalloutName", "Creator", "0.0.0")]
    public class CalloutName : Callout
    {
        public JObject GetJsonData()
        {
            string variablepath = "/callouts/Config/Config.json";
            string variabledata = API.LoadResourceFile(API.GetCurrentResourceName(), variablepath);
            JObject variablejsonData = JObject.Parse(variabledata);

            foreach (var variableDepartment in variablejsonData["CalloutName-Department"])
            {
                int.TryParse((string)variableDepartment[0], out int variabledeptID);
                _assignedDeptarments.Add(variabledeptID);
            }

            List<Vector3> variablecoords = new List<Vector3>();
            foreach (var variablecoordinate in variablejsonData["CalloutName-Coordinates"])
            {
                variablecoords.Add(JsonConvert.DeserializeObject<Vector3>(variablecoordinate.ToString()));
            }
            _variablecoordinates = variablecoords.SelectRandom();

            Dictionary<string, PedHash> variablePedHashes = new Dictionary<string, PedHash>();
            string[] variablePedJSON = JsonConvert.DeserializeObject<string[]>(variablejsonData["CalloutName-Ped"].ToString());
            foreach (string variablePedhash in variablePedJSON)
            {
                int variablePedhashKey = API.GetHashKey(variablePedhash);
                variablePedHashes.Add(variablePedhash, (PedHash)variablePedhashKey);
            }
            _variablePedHash = variablePedHashes.SelectRandom().Value;

            Dictionary<string, VehicleHash> variablevehicleHashes = new Dictionary<string, VehicleHash>();
            string[] variablevehicleJSON = JsonConvert.DeserializeObject<string[]>(variablejsonData["CalloutName-Vehicles"].ToString());
            foreach (string variablevehiclehash in variablevehicleJSON)
            {
                int variablevehiclehashKey = API.GetHashKey(variablevehiclehash);
                variablevehicleHashes.Add(variablevehiclehash, (VehicleHash)variablevehiclehashKey);
            }
            _variableVehicleHash = variablevehicleHashes.SelectRandom().Value;

            return variablejsonData;
        }

        private List<int> _assignedDeptarments = new List<int>();
        private Vector3 _variablecoordinates;
        private PedHash _variablePedHash;
        private VehicleHash _variableVehicleHash;

        private Vehicle _variableVehicle;
        Ped _variableped;

        public override async Task<bool> CheckRequirements()
        {
            var variableplayerDept = Utilities.GetPlayerData().DepartmentID;
            if (_assignedDeptarments.Contains(variableplayerDept))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CalloutName()
        {
            InitInfo(_variablecoordinates);

            ShortName = "";
            CalloutDescription = "";
            ResponseCode = ;
            StartDistance = f;
        }

        public async override void OnStart(Ped variable)
        {
            base.OnStart(variable);

            _variableped = await SpawnPed(_variablePedHash, Location);
            _variableVehicle = await SpawnVehicle(_variableVehicleHash, Location);

            PedData variablePedData = await Utilities.GetPedData(_variableped.NetworkId);
            VehicleData variableVehicleData = await Utilities.GetVehicleData(_variableVehicle.NetworkId);

            _variableped.AlwaysKeepTask = true;
            _variableped.BlockPermanentEvents = true;
            _variableped.IsPersistent = true;
            _variableVehicle.IsPersistent = true;

            Utilities.SetPedData(_variableped.NetworkId, variablePedData);
            Utilities.SetVehicleData(_variableVehicle.NetworkId, variableVehicleData);
        }

        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
        }

        public override void OnCancelAfter()
        {

        }
    }
}
