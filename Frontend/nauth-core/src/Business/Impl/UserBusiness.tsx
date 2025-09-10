import BusinessResult from '../../DTO/Business/BusinessResult';
import AuthSession from '../../DTO/Domain/AuthSession';
import UserInfo from '../../DTO/Domain/UserInfo';
import UserTokenInfo from '../../DTO/Domain/UserTokenInfo';
import IUserService from '../../Services/Interfaces/IUserService';
import UserFactory from '../Factory/UserFactory';
import IUserBusiness from '../Interfaces/IUserBusiness';

let _userService: IUserService;

const LS_KEY = 'login-with-metamask:auth';

const UserBusiness: IUserBusiness = {
  init: function (userService: IUserService): void {
    _userService = userService;
  },

  getSession: () => {
    const ls = window.localStorage.getItem(LS_KEY);
    return ls && JSON.parse(ls);
  },
  setSession: (session: AuthSession) => {
    console.log('Set Session: ', JSON.stringify(session));
    localStorage.setItem(LS_KEY, JSON.stringify(session));
  },
  cleanSession: () => {
    localStorage.removeItem(LS_KEY);
  },

  uploadImageUser: async (file: Blob) => {
    try {
      const ret = {} as BusinessResult<string>;
      const session: AuthSession = UserFactory.UserBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: 'Not logged',
        };
      }
      const retServ = await _userService.uploadImageUser(file, session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.value,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to get user by email');
    }
  },
  getMe: async () => {
    try {
      const ret = {} as BusinessResult<UserInfo>;
      const session: AuthSession = UserFactory.UserBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: 'Not logged',
        };
      }
      const retServ = await _userService.getMe(session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to get user by address');
    }
  },
  getUserByEmail: async (email: string) => {
    try {
      const ret = {} as BusinessResult<UserInfo>;
      const retServ = await _userService.getUserByEmail(email);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to get user by email');
    }
  },
  getBySlug: async (slug: string) => {
    try {
      const ret = {} as BusinessResult<UserInfo>;
      const retServ = await _userService.getBySlug(slug);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to get user by email');
    }
  },
  insert: async (user: UserInfo) => {
    try {
      const ret = {} as BusinessResult<UserInfo>;
      const retServ = await _userService.insert(user);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to insert');
    }
  },
  update: async (user: UserInfo) => {
    try {
      const ret = {} as BusinessResult<UserInfo>;
      const session: AuthSession = UserFactory.UserBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: 'Not logged',
        };
      }
      const retServ = await _userService.update(user, session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.user,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to update');
    }
  },
  loginWithEmail: async (email: string, password: string) => {
    try {
      const ret = {} as BusinessResult<UserTokenInfo>;
      const retServ = await _userService.loginWithEmail(email, password);
      if (retServ.sucesso) {
        const userTokenInfo: UserTokenInfo = {
          token: retServ.token,
          user: retServ.user
        } as UserTokenInfo;
        return {
          ...ret,
          dataResult: userTokenInfo,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to login with email');
    }
  },
  hasPassword: async () => {
    try {
      const ret = {} as BusinessResult<boolean>;
      const session: AuthSession = UserFactory.UserBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: 'Not logged',
        };
      }
      const retServ = await _userService.hasPassword(session.token);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: true,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to change password');
    }
  },
  changePassword: async (oldPassword: string, newPassword: string) => {
    const ret = {} as BusinessResult<boolean>;
    const session: AuthSession = UserFactory.UserBusiness.getSession();
    if (!session) {
      return {
        ...ret,
        sucesso: false,
        mensagem: 'Not logged',
      };
    }
    const retServ = await _userService.changePassword(oldPassword, newPassword, session.token);
    if (retServ.sucesso) {
      return {
        ...ret,
        dataResult: true,
        sucesso: true,
        mensagem: retServ.mensagem,
      };
    } else {
      return {
        ...ret,
        sucesso: false,
        mensagem: retServ.mensagem,
      };
    }
  },
  sendRecoveryEmail: async (email: string) => {
    try {
      const ret = {} as BusinessResult<boolean>;
      const retServ = await _userService.sendRecoveryEmail(email);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to send recovery email');
    }
  },
  changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
    try {
      const ret = {} as BusinessResult<boolean>;
      const retServ = await _userService.changePasswordUsingHash(recoveryHash, newPassword);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: ret.sucesso,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to change password using hash');
    }
  },
  list: async (take: number) => {
    try {
      const ret = {} as BusinessResult<UserInfo[]>;
      const retServ = await _userService.list(take);
      if (retServ.sucesso) {
        return {
          ...ret,
          dataResult: retServ.users,
          sucesso: true,
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem,
        };
      }
    } catch {
      throw new Error('Failed to get user by email');
    }
  },
  /*,
  search: async (networkId: number, keyword: string, pageNum: number, profileId?: number) => {
      let ret = {} as BusinessResult<UserListPagedInfo>;
      let session: AuthSession = AuthFactory.AuthBusiness.getSession();
      if (!session) {
        return {
          ...ret,
          sucesso: false,
          mensagem: "Not logged"
        };
      }
      let retServ = await _userService.search(networkId, keyword, pageNum, session.token, profileId);
      if (retServ.sucesso) {
        let dataResult: UserListPagedInfo;
        return {
          ...ret,
          dataResult: {
            ...dataResult,
            users: retServ.users,
            pageNum: retServ.pageNum,
            pageCount: retServ.pageCount
          },
          sucesso: true
        };
      } else {
        return {
          ...ret,
          sucesso: false,
          mensagem: retServ.mensagem
        };
      }
  }
      */
};

export default UserBusiness;
