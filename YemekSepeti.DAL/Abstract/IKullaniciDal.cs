using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.Abstract
{
    public interface IKullaniciDal: IGenericDal<Kullanici>
    {
        // Email ve şifreye göre kullanıcıyı getirir
        Kullanici? GetUserByCredentials(string email, string sifre);
    }
}
