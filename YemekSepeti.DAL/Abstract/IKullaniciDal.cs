using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.Abstract
{
    // IgenericDal dan miras aldırdık
    public interface IKullaniciDal: IGenericDal<Kullanici>
    {
        // 🔑 Login işlemi için sadece e-posta ve şifre ile sorgulama yapar.
        Kullanici? GetUserByCredentials(string email, string sifre);
    }
}
