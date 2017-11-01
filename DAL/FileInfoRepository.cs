using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAL
{
    internal class FileInfoRepository :BaseRepository<tb_FileInfo>, InterfaceFileInfoRepository
    {
    }
}