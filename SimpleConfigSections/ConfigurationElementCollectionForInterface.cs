﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleConfigSections
{
    internal class ConfigurationElementCollectionForInterface : ConfigurationElementCollection, IConfigValue
    {
        private readonly Type _elementType;
        private readonly Type _listType;
        private readonly CacheCallback<int, IList> _list;
        
        protected ConfigurationElementCollectionForInterface(Type elementType)
        {
            _elementType = elementType;
            
            _listType = typeof (List<>).MakeGenericType(new[]
                                                            {
                                                                _elementType
                                                            });
            _list = new CacheCallback<int, IList>(ignored=> CreateElements());            
        }


        public object Value(string propertyName)
        {
            return _list.Get(0).GetEnumerator();
        }

        private IList CreateElements()
        {
            var elementList = (IList) Activator.CreateInstance(_listType);
            foreach (IConfigValue configValue in this)
            {
                var obj = new ConcreteConfiguration(configValue).ClientValue(_elementType);
                elementList.Add(obj);
            }
            return elementList;
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationElementForInterface(_elementType);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid();
        }
    }   

    internal class ConfigurationElementCollectionForInterface<T> : ConfigurationElementCollectionForInterface
    {
        public ConfigurationElementCollectionForInterface()
            : base(typeof (T))
        {
        }
    }
}