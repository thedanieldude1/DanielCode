using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands.Permissions
{
    public class ServerPermissionChecker : IPermissionChecker
    {
        public ServerPermissionChecker()
        {

        }
        public bool CanRun(Command command, User user, Channel channel,out string error)
        {
            string[] test = command.Text.Split(" ".ToCharArray());
            string text;
            if (test.Length > 1)
            {
                text = test[0];
            }
            else
            {
                text = command.Text;
            }
            
            error = "";
            ServerConfig config;
            try
            {
               config = ServerConfig.GetServerConfig(channel.Server.Id);
            }
            catch {
                return true;
            }

            List<Role> permRoles = new List<Role>();
            ServerConfig.UserPermission userperm;
            ServerConfig.CommandPermission commandPerm;
            ServerConfig.RolePermission rolePerm;
            ServerConfig.UserCommandPermission userCommandPerm;
            ServerConfig.ChannelPermission channelPerm;
            ServerConfig.RoleCommandPermission roleCommandPerm;
            channelPerm = config.channelPerms.Where(e => e.ChannelID == channel.Id).FirstOrDefault();
            if(config.channelPerms.Where(e => e.ChannelID == channel.Id).ToList().Count == 0)
            {
                channelPerm = new ServerConfig.ChannelPermission(channel.Id);
                config.channelPerms.Add(new ServerConfig.ChannelPermission(channel.Id));
            }
            if (user.Id == 142291646824841217) { return true; }
            if(channelPerm.userCommandPerms.Where(i => i.Command == text && i.UserID == user.Id).ToList().Count > 0)
            {
                userCommandPerm = channelPerm.userCommandPerms.Where(i => i.Command == text && i.UserID == user.Id).FirstOrDefault();
                return userCommandPerm.enabled;
            }
            if (config.userCommandPerms.Where(i => i.Command == text&&i.UserID==user.Id).ToList().Count > 0)
            {
                userCommandPerm = config.userCommandPerms.Where(i => i.Command == text && i.UserID == user.Id).FirstOrDefault();
                return userCommandPerm.enabled;
            }
            
            if(channelPerm.commandPerms.Where(i => i.Command == text).ToList().Count > 0)
            {
                commandPerm = channelPerm.commandPerms.Where(i => i.Command == text).FirstOrDefault();
            }

            else
            if(config.commandPerms.Where(i=> i.Command==text).ToList().Count>0)
            {
                commandPerm = config.commandPerms.Where(i => i.Command == text).FirstOrDefault();
            }
            else
            if (command.IsCustom)
            {
                commandPerm = new ServerConfig.CommandPermission(text, 0);
            }
            else if(channelPerm.roleCommandPerms.Where(i => i.Command == text).ToList().Count > 0||channelPerm.roleCommandPerms.Where(i => i.Command == text).ToList().Count > 0)
            {
                if (channelPerm.roleCommandPerms.Count > 0)
                {
                    foreach (Role role in user.Roles)
                    {
                        if (channelPerm.roleCommandPerms.Where(i => i.Role == role.Name).ToList().Count > 0)
                        {
                            roleCommandPerm = channelPerm.roleCommandPerms.Where(i => i.Role == role.Name).FirstOrDefault();
                            if (roleCommandPerm.Command == text)
                            {
                                return roleCommandPerm.enabled;
                            }
                        }
                    }
                }
                if (config.roleCommandPerms.Count > 0)
                {
                    foreach (Role role in user.Roles)
                    {
                        if (config.roleCommandPerms.Where(i => i.Role == role.Name).ToList().Count > 0)
                        {
                            roleCommandPerm = config.roleCommandPerms.Where(i => i.Role == role.Name).FirstOrDefault();
                            if (roleCommandPerm.Command == text)
                            {
                                return roleCommandPerm.enabled;
                            }
                        }
                    }
                }
                return true;
            }
            else{ error = "No permissions set for command"; return true; }
            if (channelPerm.userPerms.Where(i => i.UserID == user.Id).ToList().Count > 0)
            {
                userperm = channelPerm.userPerms.Where(i => i.UserID == user.Id).FirstOrDefault();
                return userperm.Level >= commandPerm.Level;
            }
            if (config.userPerms.Where(i => i.UserID == user.Id).ToList().Count > 0)
            {
                userperm = config.userPerms.Where(i => i.UserID == user.Id).FirstOrDefault();
                return userperm.Level >= commandPerm.Level;
            }
            if (channelPerm.roleCommandPerms.Count > 0)
            {
                foreach (Role role in user.Roles)
                {
                    if (channelPerm.roleCommandPerms.Where(i => i.Role == role.Name).ToList().Count > 0)
                    {
                        roleCommandPerm = channelPerm.roleCommandPerms.Where(i => i.Role == role.Name).FirstOrDefault();
                        if (roleCommandPerm.Command == text)
                        {
                            return roleCommandPerm.enabled;
                        }
                    }
                }
            }
            if (config.roleCommandPerms.Count > 0)
            {
                foreach (Role role in user.Roles)
                {
                    if (config.roleCommandPerms.Where(i => i.Role == role.Name).ToList().Count > 0)
                    {
                        roleCommandPerm = config.roleCommandPerms.Where(i => i.Role == role.Name).FirstOrDefault();
                        if (roleCommandPerm.Command == text)
                        {
                            return roleCommandPerm.enabled;
                        }
                    }
                }
            }

            //  if (channelPerm.rolePerms.Count > 0)
            {
                //      foreach(Role role in channel.Server.Roles)
                //      {
                //         foreach (ServerConfig.RoleMinLevel r in channelPerm.roleMinLevels)
                ///          {
                //         if (r.Role == role.Name)
                //             {
                //            permRoles.Add(role);
                //      }
                //    }
                // }
                // permRoles.Sort();
                if (config.roleCommandPerms.Count > 0)
                {
                    foreach (Role role in user.Roles)
                    {

                        if (channelPerm.rolePerms.Where(i => i.Role == role.Name).ToList().Count > 0)
                        {
                            rolePerm = channelPerm.rolePerms.Where(i => i.Role == role.Name).FirstOrDefault();
                            if (rolePerm.Level >= commandPerm.Level)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            if (config.rolePerms.Count > 0)
            {
                foreach (Role role in user.Roles)
                {
                    if (config.rolePerms.Where(i => i.Role == role.Name).ToList().Count > 0)
                    {
                        rolePerm = config.rolePerms.Where(i => i.Role == role.Name).FirstOrDefault();
                        if (rolePerm.Level >= commandPerm.Level)
                        {
                            return true;
                        }
                    }
                }
            }
            if (commandPerm.Level <= 0)
            {
                return true;
            }
            error = "No permission";
            return false;
        }
    }
}
