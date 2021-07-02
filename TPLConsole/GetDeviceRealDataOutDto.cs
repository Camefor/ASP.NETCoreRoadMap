using System;

namespace TPLConsole
{

    public class OutBaseDto<T>
    {
        /// <summary>
        /// 响应码，非0即为失败
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 响应描述
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public T data { get; set; }
    }


    /// <summary>
    /// 获取站点（设备）最新监测数据(建筑工地)
    /// </summary>
    public class GetDeviceRealDataOutDto
    {
        /// <summary>
        /// 设备MN码
        /// </summary>
        public string mnCode { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string siteName { get; set; }
        /// <summary>
        /// 数据时间
        /// </summary>
        public DateTime dataTime { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public double dust { get; set; }
        /// <summary>
        /// 温度标志
        /// </summary>
        public string dustFlag { get; set; }
        /// <summary>
        /// 湿度
        /// </summary>
        public double humidity { get; set; }
        /// <summary>
        /// 湿度标志
        /// </summary>
        public string humidityFlag { get; set; }
        /// <summary>
        /// 大气压
        /// </summary>
        public double temperature { get; set; }
        /// <summary>
        /// 大气压标志
        /// </summary>
        public string temperatureFlag { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public double windSpeed { get; set; }
        /// <summary>
        /// 风速标志
        /// </summary>
        public string windSpeedFlag { get; set; }
        /// <summary>
        /// 风向
        /// </summary>
        public double windDirection { get; set; }
        /// <summary>
        /// 风向标志
        /// </summary>
        public string windDirectionFlag { get; set; }
        /// <summary>
        /// 粉尘
        /// </summary>
        public double atm { get; set; }
        /// <summary>
        /// 粉尘标志
        /// </summary>
        public string atmFlag { get; set; }
    }
}
