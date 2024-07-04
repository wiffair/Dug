// Dug is a DNS lookup tool
// Copyright(C) 2024  Richard Cole
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

using dug.models;

namespace dug.dataContainers
{
    public class DataContainer
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private List<DnsTestRecord> _dnsTestRecords = new List<DnsTestRecord>();
        private int _index = -1;
        private ushort _idCount = 0x0001;
        private Dictionary<ushort,bool> _responseReceived = new Dictionary<ushort,bool>();

        public int AddData(DnsTestRecord response)
        {
            _lock.EnterWriteLock();
            try
            {
                _dnsTestRecords.Add(response);
                _responseReceived.Add(_idCount, false);
                _index++;
                _idCount++;
                
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            return _index;
        }

        public bool ResponseReceived(ushort id)
        {
            return _responseReceived.GetValueOrDefault(id);
        }

        public void Timeout(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                DnsTestRecord query = _dnsTestRecords[index];
                query.TimeOut = true;
                _dnsTestRecords[index] = query;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        public void MergeData(DnsTestRecord response)
        {
            ushort idNumber = response.ResponseHeader.Id;
            _lock.EnterWriteLock();
            try
            {
                _responseReceived[idNumber] = true;
                int index = _dnsTestRecords.FindIndex(r => r.QueryHeader.Id == idNumber);
                //Console.WriteLine($"DC -- Index: {index} : ID {idNumber}");
                DnsTestRecord query = _dnsTestRecords[index];
                query.Receive=true;
                response.Visable = query.Visable;
                response.QueryDateTime = query.QueryDateTime;
                response.QueryTime = response.ResponseDateTime.Subtract(query.QueryDateTime);
                response.QueryByteCount=query.QueryByteCount;
                response.QueryRawData= query.QueryRawData;
                response.QueryHeader= query.QueryHeader;
                response.QueryQuestion= query.QueryQuestion;
                response.QueryServer= query.QueryServer;
                _dnsTestRecords[index]=response;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

        }
        public DnsTestRecord ReadData(int index)
        {
            _lock.EnterReadLock();
            try
            {
                return _dnsTestRecords[index];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<DnsTestRecord> ReadAllData()
        {
            _lock.EnterReadLock();
            try
            {
                return _dnsTestRecords;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        public ushort GetIdCount()
        {
            return _idCount;
        }
    }

}
