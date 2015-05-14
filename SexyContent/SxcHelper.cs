using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent
{
	public class SxcHelper
	{
		private readonly SexyContent Sexy;
		public SxcHelper(SexyContent sexy)
		{
			Sexy = sexy;
		}

		private Serializer _serializer;
		public Serializer Serializer
		{
			get {
			    if (_serializer == null)
			    {
			        _serializer = new Serializer();
			        _serializer.Sxc = Sexy;
			    }
			    return _serializer;
			}
		}
	}
}