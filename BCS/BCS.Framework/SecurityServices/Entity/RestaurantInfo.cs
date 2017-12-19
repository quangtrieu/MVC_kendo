using System;
using System.ComponentModel;
using System.Xml.Serialization;
using BCS.Framework.Constants;
using BCS.Framework.Commons;

namespace BCS.Framework.SecurityServices.Entity {

    public class RestaurantInfo : AuditableEntity 
    {
        public int RestID { get; set; }

        public string RestName { get; set; }

        public string RestCode { get; set; }

        public int StatusID { get; set; }

        public int AddressID { get; set; }

        public int TimeZoneID { get; set; }

        public int otTypeID { get; set; }

        public double otRate { get; set; }

        public double ShiftHrs { get; set; }

        [XmlIgnore]
        public TimeSpan CutOffTime {
            get
            {
                if (CutOffTimeDisp == null || CutOffTimeDisp.Equals(""))
                {
                    return TimeSpan.Parse(DateTime.Now.ToString(Constant.TIME_SPAN_FORMAT));
                }
                else
                {
                    return TimeSpan.Parse(CutOffTimeDisp);
                }
            }
            set
            {
                CutOffTimeDisp = value.ToString();
            }
        }

        [Browsable(false)]
        [XmlElement("CutOffTime")]
        public string CutOffTimeDisp { get; set; }

        public int EnableEmail { get; set; }

        public bool TextAllowed { get; set; }

        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryCode { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }


        //StateCode
        public string StateCode { get; set; }
        public string StateName { get; set; }

        public string AddressFull
        {
            get
            {
                return string.Format("{0}{1}{2}{3}{4}",
                    string.IsNullOrEmpty(Street1) ? Street2 : Street1 + ", ",
                    string.IsNullOrEmpty(City) ? string.Empty : City,
                    string.IsNullOrEmpty(StateName) ? string.Empty : ", " + StateName,
                    string.IsNullOrEmpty(Zip) ? string.Empty : ", " + Zip,
                    string.IsNullOrEmpty(CountryCode) ? string.Empty : ", " + CountryCode);
            }
        }
    }
}
