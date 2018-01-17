
local UIBase = import('KSFramework/UIBase')

if not Cookie then local Cookie = Slua.GetClass('KSFramework.Cookie') end
if not Log then Log = Slua.GetClass('KEngine.Log') end

local UINewUI_p4wg1fxh.jt8 = {}
extends(UINewUI_p4wg1fxh.jt8, UIBase)

-- create a ui instance
function UINewUI_p4wg1fxh.jt8.New(controller)
    local newUI = new(UINewUI_p4wg1fxh.jt8)
    newUI.Controller = controller
    return newUI
end

function UINewUI_p4wg1fxh.jt8:OnInit(controller)
    Log.Info('UINewUI_p4wg1fxh.jt8 OnInit, do controls binding')
end

function UINewUI_p4wg1fxh.jt8:OnOpen()
    Log.Info('UINewUI_p4wg1fxh.jt8 OnOpen, do your logic')
end

return UINewUI_p4wg1fxh.jt8
