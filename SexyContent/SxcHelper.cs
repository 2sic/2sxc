using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent
{
	public class SxcHelper
	{
		private readonly SxcInstance Sexy;
		public SxcHelper(SxcInstance sexy)
		{
			Sexy = sexy;
		}

		private Serializer _serializer;
		public Serializer Serializer
		{
			get {
			    if (_serializer == null)
			        _serializer = new Serializer(Sexy);
			    return _serializer;
			}
		}
	}
}