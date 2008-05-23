using System;

namespace Northwind.DAL
{
	/// <summary>
	/// This object represents the properties and methods of a Employee.
	/// </summary>
	public class Employee
	{
		private Int32 _employeeID;
		private String _lastName;
		private String _firstName;
		private String _title;
		private String _titleOfCourtesy;
		private DateTime _birthDate;
		private DateTime _hireDate;
		private String _address;
		private String _city;
		private String _region;
		private String _postalCode;
		private String _country;
		private String _homePhone;
		private String _extension;
		private Byte[] _photo;
		private String _notes;
		private Int32 _reportsTo;
		private String _photoPath;
		
		public Employee()
		{
		}
		
		#region Custom - Methods
		// Insert custom methods in here so that they are preserved during re-generation.
		#endregion
	}
}
