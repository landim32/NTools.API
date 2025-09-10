import { useState } from 'react';
import IUserProvider from './IUserProvider';
import UserContext from './UserContext';
import UserInfo from '../../DTO/Domain/UserInfo';
import ProviderResult from '../../DTO/Contexts/ProviderResult';
import UserFactory from '../../Business/Factory/UserFactory';
import UserProviderResult from '../../DTO/Contexts/UserProviderResult';
import UrlProviderResult from '../../DTO/Contexts/UrlProviderResult';
import { LanguageEnum } from '../../DTO/Enum/LanguageEnum';
import AuthSession from '../../DTO/Domain/AuthSession';

export default function UserProvider(props: any) {
  const [loading, setLoading] = useState<boolean>(false);
  const [loadingList, setLoadingList] = useState<boolean>(false);
  const [loadingPassword, setLoadingPassword] = useState<boolean>(false);
  const [loadingUpdate, setLoadingUpdate] = useState<boolean>(false);
  const [loadingSearch, setLoadingSearch] = useState<boolean>(false);

  const [language, setLanguage] = useState<LanguageEnum>(LanguageEnum.English);
  const [sessionInfo, _setSessionInfo] = useState<AuthSession>(null);

  const [userHasPassword, setUserHasPassword] = useState<boolean>(false);

  const [userImage, setUserImage] = useState<string>('');
  const [user, _setUser] = useState<UserInfo>(null);
  const [users, setUsers] = useState<UserInfo[]>([]);

  const userProviderValue: IUserProvider = {
    loading: loading,
    loadingList: loadingList,
    loadingPassword: loadingPassword,
    loadingUpdate: loadingUpdate,
    loadingSearch: loadingSearch,
    userHasPassword: userHasPassword,
    user: user,
    users: users,

    language: language,
    sessionInfo: sessionInfo,

    setSession: (session: AuthSession) => {
      console.log(JSON.stringify(session));
      _setSessionInfo(session);
      UserFactory.UserBusiness.setSession(session);
    },
    setLanguage: (value: LanguageEnum) => {
      setLanguage(value);
    },
    setUser: (user: UserInfo) => {
      _setUser(user);
    },


    logout: function (): ProviderResult {
      try {
        UserFactory.UserBusiness.cleanSession();
        _setSessionInfo(null);
        return {
          sucesso: true,
          mensagemErro: '',
          mensagemSucesso: '',
        };
      } catch (err) {
        return {
          sucesso: false,
          mensagemErro: 'Falha ao tenta executar o logout',
          mensagemSucesso: '',
        };
      }
    },
    loadUserSession: async () => {
      const ret = {} as Promise<ProviderResult>;
      const session = await UserFactory.UserBusiness.getSession();
      if (session) {
        userProviderValue.setSession(session);
        return {
          ...ret,
          sucesso: true,
        };
      }
      return {
        ...ret,
        sucesso: false,
        mensagemErro: 'Session not load',
      };
    },

    uploadImageUser: async (file: Blob) => {
      const ret = {} as Promise<UrlProviderResult>;
      setLoading(true);
      const brt = await UserFactory.UserBusiness.uploadImageUser(file);
      if (brt.sucesso) {
        setLoading(false);
        setUserImage(brt.dataResult);
        return {
          ...ret,
          sucesso: true,
          url: brt.dataResult,
          mensagemSucesso: 'Profile added',
        };
      } else {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: brt.mensagem,
        };
      }
    },
    getMe: async () => {
      const ret = {} as Promise<UserProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.getMe();
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            user: brt.dataResult,
            sucesso: true,
            mensagemSucesso: 'User load',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            user: null,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          user: null,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    getUserByEmail: async (email: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.getUserByEmail(email);
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User load',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    getBySlug: async (slug: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const brt = await UserFactory.UserBusiness.getBySlug(slug);
        if (brt.sucesso) {
          setLoading(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User load',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    insert: async (user: UserInfo) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.insert(user);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User inseted',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    update: async (user: UserInfo) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.update(user);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          _setUser(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User updated',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    loginWithEmail: async (email: string, password: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoading(true);
      try {
        const bsRet = await UserFactory.UserBusiness.loginWithEmail(email, password);
        if (bsRet.sucesso) {
          setLoading(false);
          _setUser(bsRet.dataResult.user);
          userProviderValue.setSession({
            ...sessionInfo,
            userId: bsRet.dataResult.user.userId,
            hash: bsRet.dataResult.user.hash,
            token: bsRet.dataResult.token,
            isAdmin: bsRet.dataResult.user.isAdmin,
            name: bsRet.dataResult.user.name,
            email: bsRet.dataResult.user.email,
            language: language,
          });
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'User successfully logged',
          };
        } else {
          setLoading(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: bsRet.mensagem,
          };
        }
      } catch (err) {
        setLoading(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    hasPassword: async () => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingPassword(true);
      setUserHasPassword(false);
      try {
        const brt = await UserFactory.UserBusiness.hasPassword();
        if (brt.sucesso) {
          setUserHasPassword(true);
          setLoadingPassword(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'Password changed',
          };
        } else {
          setLoadingPassword(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingPassword(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    changePassword: async (oldPassword: string, newPassword: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.changePassword(oldPassword, newPassword);
        console.log('changePassword: ', JSON.stringify(brt));
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: brt.mensagem,
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        console.log('Error change password: ', err);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    sendRecoveryEmail: async (email: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.sendRecoveryEmail(email);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'Recovery email sent successfully',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingUpdate(true);
      try {
        const brt = await UserFactory.UserBusiness.changePasswordUsingHash(recoveryHash, newPassword);
        if (brt.sucesso) {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: true,
            mensagemSucesso: 'Recovery email sent successfully',
          };
        } else {
          setLoadingUpdate(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingUpdate(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    },
    list: async (take: number) => {
      const ret = {} as Promise<ProviderResult>;
      setLoadingList(true);
      try {
        const brt = await UserFactory.UserBusiness.list(take);
        if (brt.sucesso) {
          setLoadingList(false);
          setUsers(brt.dataResult);
          return {
            ...ret,
            sucesso: true,
          };
        } else {
          setLoadingList(false);
          return {
            ...ret,
            sucesso: false,
            mensagemErro: brt.mensagem,
          };
        }
      } catch (err) {
        setLoadingList(false);
        return {
          ...ret,
          sucesso: false,
          mensagemErro: JSON.stringify(err),
        };
      }
    }
  };

  return <UserContext.Provider value={userProviderValue}>{props.children}</UserContext.Provider>;
}
