﻿using BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITagRepository
    {
        void Add(Tag tag);
        List<Tag> GetAllTag();
        Tag? GetTagById(int tagId);
    }
}
