﻿/*
 * Copyright (c) 2015 Rafael Baquero
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Job_Hunter.Model
{
    public class JobTypeItem
    {
        private long _id;
        private long _position;
        private string _jobType;
        private string _note;

        public JobTypeItem(long id, long position, string jobType, string note)
        {
            _id = id;
            _position = position;
            _jobType = jobType;
            _note = note;
        }

        public long Id
        {
            get { return _id; }
        }

        public long Position
        {
            get { return _position; }
        }

        public string JobType
        {
            get { return _jobType; }
        }

        public string Note
        {
            get { return _note; }
        }
    }
}