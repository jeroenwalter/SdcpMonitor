// ReSharper disable UnusedMember.Global
// // ReSharper disable IdentifierTypo
// // ReSharper disable InconsistentNaming
namespace Sdcp;

public struct AttributeMessage
{
  /*
  {
    "Attributes": {
        "Name": "PrinterName",  // Machine Name
        "MachineName": "MachineModel",  // Machine Model
        "BrandName": "CBD",  // Brand Name
        "ProtocolVersion": "V3.0.0",  // Protocol Version
        "FirmwareVersion": "V1.0.0",  // Firmware Version
        "Resolution": "7680x4320",  // Resolution
        "XYZsize": "210x140x100" ,  // Maximum printing dimensions in the XYZ directions of the machine, in millimeters.(mm)
        "MainboardIP": "192.168.1.1",  // Motherboard IP Address
        "MainboardID": "000000000001d354", // Motherboard ID(16)
        "NumberOfVideoStreamConnected": 1,  // Number of Connected Video Streams
        "MaximumVideoStreamAllowed": 1,  // Maximum Number of Connections for Video Streams
        "NetworkStatus": "'wlan' | 'eth'",  // Network Connection Status, WiFi/Ethernet Port
        "UsbDiskStatus": 0,  // USB Drive Connection Status. 0: Disconnected, 1: Connected
        "Capabilities":[
            "FILE_TRANSFER",  // Support File Transfer
            "PRINT_CONTROL",  // Support Print Control
            "VIDEO_STREAM"  // Support Video Stream Transmission
        ],  // Supported Sub-protocols on the Motherboard
        "SupportFileType":[
            "CTB"  // Supports CTB File Type
        ],
        //Device Self-Check Status
        "DevicesStatus":{
            "TempSensorStatusOfUVLED": 0, // UVLED Temperature Sensor Status, 0: Disconnected, 1: Normal, 2: Abnormal
            "LCDStatus": 0,  // Exposure Screen Connection Status, 0: Disconnected, 1: Connected
            "SgStatus": 0,  // Strain Gauge Status, 0: Disconnected, 1: Normal, 2: Calibration Failed
            "ZMotorStatus": 0,  // Z-Axis Motor Connection Status, 0: Disconnected, 1: Connected
            "RotateMotorStatus": 0,  // Rotary Axis Motor Connection Status, 0: Disconnected, 1: Connected
            "RelaseFilmState": 0,  // Release Film Status, 0: Abnormal, 1: Normal
            "XMotorStatus": 0  // X-Axis Motor Connection Status, 0: Disconnected, 1: Connected
        },
        "ReleaseFilmMax": 0,  // Maximum number of uses (service life) for the release film
        "TempOfUVLEDMax": 0,  // Maximum operating temperature for UVLED(℃)
        "CameraStatus": 0,  // Camera Connection Status, 0: Disconnected, 1: Connected 
        "RemainingMemory": 123455,  // Remaining File Storage Space Size(bit)
        "TLPNoCapPos": 50.0,  // Model height threshold for not performing time-lapse photography (mm)
        "TLPStartCapPos": 30.0,  // The print height at which time-lapse photography begins (mm)
        "TLPInterLayers": 20  // Time-lapse photography shooting interval layers    
    }, 
    "MainboardID":"ffffffff",  // Motherboard ID
    "TimeStamp":1687069655,  // Timestamp
    "Topic":"sdcp/attributes/${MainboardID}"  // Topic, used to distinguish the type of reported message
}
  */
}