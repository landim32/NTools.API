import { Link, useLocation } from "react-router-dom"
import { ThemeToggle } from "./ThemeToggle"
import { MobileNav } from "./MobileNav"
import { cn } from "@/lib/utils"
import nauthLogo from "@/assets/nauth-logo.png"
import { useContext, useEffect } from "react";
import { toast } from "sonner";
import { User } from "lucide-react"
import { IUserProvider, UserContext } from "@/lib/nauth-core"

const navigationItems = [
  { href: "/login", label: "Login" },
  { href: "/signup", label: "Simple Signup" },
  { href: "/register", label: "Full Registration" },
  { href: "/change-password", label: "Change Password" },
  { href: "/recover-password", label: "Recover Password" },
]

export function Navigation() {

  const location = useLocation();

  const userContext = useContext<IUserProvider>(UserContext);

  useEffect(() => {
    userContext.loadUserSession().then((ret) => {
      if (!ret.sucesso) {
        toast.error(ret.mensagemErro);
      }
    });
  }, []);

  return (
    <header className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container flex h-16 max-w-screen-2xl items-center">
        <div className="mr-4 hidden md:flex">
          <Link to="/" className="mr-6 flex items-center space-x-3">
            <img src={nauthLogo} alt="NAuth Logo" className="h-8 w-8" />
            <div className="flex flex-col">
              <span className="text-lg font-bold bg-gradient-to-r from-brand-primary to-brand-secondary bg-clip-text text-transparent">
                NAuth
              </span>
              <span className="text-xs text-muted-foreground hidden sm:block">
                Secure Authentication
              </span>
            </div>
          </Link>
          <nav className="flex items-center gap-6 text-sm">
            {navigationItems.map((item) => (
              <Link
                key={item.href}
                to={item.href}
                className={cn(
                  "transition-colors hover:text-foreground/80",
                  location.pathname === item.href
                    ? "text-foreground"
                    : "text-foreground/60"
                )}
              >
                {item.label}
              </Link>
            ))}
            {userContext.sessionInfo &&
              <Link
                to="/login"
                className="transition-colors hover:text-foreground/80 text-foreground/60"
                onClick={async () => { await userContext.logout() }}
              >
                Logoff
              </Link>
            }
          </nav>
        </div>

        {/* Mobile Navigation */}
        <div className="flex flex-1 items-center justify-between space-x-2 md:hidden">
          <Link to="/" className="flex items-center space-x-2">
            <img src={nauthLogo} alt="NAuth Logo" className="h-7 w-7" />
            <span className="text-lg font-bold bg-gradient-to-r from-brand-primary to-brand-secondary bg-clip-text text-transparent">
              NAuth
            </span>
          </Link>
          <MobileNav />
        </div>

        <div className="flex flex-1 items-center justify-end space-x-4">
          <nav className="flex items-center">
            {userContext.sessionInfo &&
              <Link to="/login" className="flex items-center mr-2">
                <User className="h-5 w-5" />
                {userContext.sessionInfo.name}
              </Link>
            }
            <ThemeToggle />
          </nav>
        </div>
      </div>
    </header>
  )
}